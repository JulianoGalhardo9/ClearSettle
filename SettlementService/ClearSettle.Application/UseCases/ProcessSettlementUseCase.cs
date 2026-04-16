using ClearSettle.Domain.Entities; 
using ClearSettle.Domain.Interfaces;

namespace ClearSettle.Application.UseCases
{
    public class ProcessSettlementUseCase
    {
        private readonly ITradeRepository _tradeRepository;

        public ProcessSettlementUseCase(ITradeRepository tradeRepository)
        {
            _tradeRepository = tradeRepository;
        }

        public async Task<IEnumerable<Trade>> ExecuteAsync()
        {
            var cutoffDate = DateTime.UtcNow.AddMinutes(-1); 
            var pendingTrades = await _tradeRepository.GetPendingTradesForSettlementAsync(cutoffDate);
            var processedTrades = new List<Trade>();

            foreach (var trade in pendingTrades)
            {
                try
                {
                    trade.MarkAsSettled(); 
                    await _tradeRepository.UpdateAsync(trade);
                    processedTrades.Add(trade); 
                }
                catch (Exception)
                {
                    trade.MarkAsFailed();
                    await _tradeRepository.UpdateAsync(trade);
                }
            }

            return processedTrades; 
        }
    }
}