using System;
using System.Threading.Tasks;
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

        public async Task ExecuteAsync()
        {
            var cutoffDate = DateTime.UtcNow.AddMinutes(-1); 

            var pendingTrades = await _tradeRepository.GetPendingTradesForSettlementAsync(cutoffDate);

            foreach (var trade in pendingTrades)
            {
                try
                {
                    trade.MarkAsSettled(); 
                }
                catch (Exception)
                {
                    trade.MarkAsFailed();
                }

                await _tradeRepository.UpdateAsync(trade);
            }
        }
    }
}