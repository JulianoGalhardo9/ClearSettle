using ClearSettle.Domain.Entities;

namespace ClearSettle.Domain.Interfaces
{
    public interface ITradeRepository
    {
        Task<Trade?> GetByIdAsync(Guid id); 

        Task AddAsync(Trade trade); 

        Task UpdateAsync(Trade trade); 

        Task<IEnumerable<Trade>> GetPendingTradesForSettlementAsync(DateTime limitDate);
    }
}