using System.Text;
using System.Text.Json;
using ClearSettle.Application.DTOs;
using ClearSettle.Application.UseCases;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ClearSettle.Worker
{
    public class TradeWorker : BackgroundService
    {
        private readonly ILogger<TradeWorker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private IConnection? _connection;
        private IChannel? _channel;
        private const string QueueName = "trade_pending_queue";

        public TradeWorker(ILogger<TradeWorker> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                Port = 5672,
                UserName = "admin",
                Password = "admin123"
            };

            _connection = await factory.CreateConnectionAsync(stoppingToken);
            _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

            await _channel.QueueDeclareAsync(queue: QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null, cancellationToken: stoppingToken);

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                try
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var input = JsonSerializer.Deserialize<RegisterTradeInput>(message, options);

                    if (input != null)
                    {
                        using (var scope = _serviceProvider.CreateScope())
                        {
                            var useCase = scope.ServiceProvider.GetRequiredService<RegisterPendingTradeUseCase>();
                            await useCase.ExecuteAsync(input);
                        }
                    }

                    _logger.LogInformation($"Operação recebida via RabbitMQ e salva: {message}");

                    await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Erro ao processar mensagem do RabbitMQ: {ex.Message}");
                    await _channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true, cancellationToken: stoppingToken);
                }
            };

            await _channel.BasicConsumeAsync(queue: QueueName, autoAck: false, consumer: consumer, cancellationToken: stoppingToken);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_channel != null) await _channel.CloseAsync(cancellationToken);
            if (_connection != null) await _connection.CloseAsync(cancellationToken);
            await base.StopAsync(cancellationToken);
        }
    }
}