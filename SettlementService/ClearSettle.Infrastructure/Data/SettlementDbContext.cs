using ClearSettle.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ClearSettle.Infrastructure.Data
{
    public class SettlementDbContext : DbContext
    {
        public SettlementDbContext(DbContextOptions<SettlementDbContext> options) : base(options)
        {
        }

        public DbSet<Trade> Trades { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            
            base.OnModelCreating(modelBuilder);
        }
    }
}