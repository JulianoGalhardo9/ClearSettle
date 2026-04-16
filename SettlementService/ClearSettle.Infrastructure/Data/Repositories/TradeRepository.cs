using ClearSettle.Domain.Entities; 
using ClearSettle.Domain.Interfaces; 

namespace ClearSettle.Infrastructure.Data.Repositories
{
    public class TradeRepository : ITradeRepository
    {
        private readonly SettlementDbContext _context;

        public TradeRepository(SettlementDbContext context)
        {
            _context = context;
        }

        public async Task<Trade?> GetByIdAsync(Guid id)
        {
            return await _context.Trades.FindAsync(id);
        }

        public async Task AddAsync(Trade trade)
        {
            await _context.Trades.AddAsync(trade);
            
            await _context.SaveChangesAsync();
        }


        public async Task UpdateAsync(Trade trade)
        {
            _context.Trades.Update(trade);
            
            await _context.SaveChangesAsync();
        }
    }
}