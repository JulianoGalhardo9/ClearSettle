using ClearSettle.Application.UseCases;

namespace ClearSettle.Worker
{
    public class SettlementJobWorker : BackgroundService
    {
        private readonly ILogger<SettlementJobWorker> _logger;
        private readonly IServiceProvider _serviceProvider;

        public SettlementJobWorker(ILogger<SettlementJobWorker> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(15));

            while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            {
                _logger.LogInformation($"[{DateTime.UtcNow:HH:mm:ss}] Iniciando varredura de liquidações D+2...");

                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var useCase = scope.ServiceProvider.GetRequiredService<ProcessSettlementUseCase>();
                        var settledTrades = await useCase.ExecuteAsync();

                        // SE houver operações liquidadas, avisa o mundo (A API)
                        if (settledTrades.Any())
                        {
                            // Importante: Adicione `using ClearSettle.Infrastructure.Messaging;` lá no topo do arquivo!
                            var publisher = new ClearSettle.Infrastructure.Messaging.RabbitMqPublisher();
                            
                            foreach (var trade in settledTrades)
                            {
                                // Publica em uma fila nova, exclusiva para avisos de status
                                await publisher.PublishAsync(trade, "trade_settled_queue");
                                _logger.LogInformation($"Aviso de liquidação enviado para a fila: {trade.Id}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao processar liquidações D+2");
                }
            }
        }
    }
}