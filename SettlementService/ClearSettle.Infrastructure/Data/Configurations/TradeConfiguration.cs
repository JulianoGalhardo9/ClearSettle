using ClearSettle.Domain.Entities; 
using Microsoft.EntityFrameworkCore; 
using Microsoft.EntityFrameworkCore.Metadata.Builders; 

namespace ClearSettle.Infrastructure.Data.Configurations
{
    public class TradeConfiguration : IEntityTypeConfiguration<Trade>
    {
        public void Configure(EntityTypeBuilder<Trade> builder)
        {
            builder.ToTable("Trades");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.TickerSymbol)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(t => t.Quantity)
                .IsRequired();

            builder.Property(t => t.Price)
                .IsRequired()
                .HasColumnType("decimal(18,8)");

            builder.Property(t => t.BuyerAccountId).IsRequired();
            builder.Property(t => t.SellerAccountId).IsRequired();
            builder.Property(t => t.TradeDate).IsRequired();
            
            builder.Property(t => t.Status)
                .IsRequired()
                .HasMaxLength(20);
        }
    }
}