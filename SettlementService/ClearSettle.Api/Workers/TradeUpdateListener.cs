using System.Text;
using ClearSettle.Api.Hubs;
using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ClearSettle.Api.Workers
{
    public class TradeUpdateListener : BackgroundService
    {
        private readonly IHubContext<TradeHub> _hubContext;
        private IConnection? _connection;
        private IChannel? _channel;

        public TradeUpdateListener(IHubContext<TradeHub> hubContext)
        {
            _hubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory() { HostName = "localhost", Port = 5672, UserName = "admin", Password = "admin123" };
            _connection = await factory.CreateConnectionAsync(stoppingToken);
            _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

            await _channel.QueueDeclareAsync(queue: "trade_settled_queue", durable: true, exclusive: false, autoDelete: false, arguments: null, cancellationToken: stoppingToken);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var messageJson = Encoding.UTF8.GetString(body);

                await _hubContext.Clients.All.SendAsync("ReceiveTradeUpdate", messageJson, cancellationToken: stoppingToken);

                await _channel.BasicAckAsync(ea.DeliveryTag, false, stoppingToken);
            };

            await _channel.BasicConsumeAsync("trade_settled_queue", false, consumer, stoppingToken);
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}