using ClearSettle.Domain.Entities; 
using ClearSettle.Domain.Interfaces; 
using Microsoft.EntityFrameworkCore;

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

        public async Task<IEnumerable<Trade>> GetPendingTradesForSettlementAsync(DateTime limitDate)
        {
            return await _context.Trades
                .Where(t => t.Status == "Pending" && t.TradeDate <= limitDate)
                .ToListAsync();
        }
    }
}