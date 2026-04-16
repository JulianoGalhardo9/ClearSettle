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
                        
                        await useCase.ExecuteAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Erro crítico no Job de Liquidação: {ex.Message}");
                }
            }
        }
    }
}