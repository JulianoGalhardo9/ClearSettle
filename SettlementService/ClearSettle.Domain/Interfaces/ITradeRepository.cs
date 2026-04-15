namespace ClearSettle.Domain.Interfaces
{
    public interface ITradeRepository
    {
        Task<Trade?> GetByIdAsync(Guid id); 

        Task AddAsync(Trade trade); 

        Task UpdateAsync(Trade trade); 
    }
}