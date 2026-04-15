namespace ClearSettle.Domain.Entities
{
    public class Trade
    {
        public Guid Id { get; private set; }
        
        public string TickerSymbol { get; private set; }
        
        public int Quantity { get; private set; }
        
        public decimal Price { get; private set; } 
        
        public Guid BuyerAccountId { get; private set; }
        
        public Guid SellerAccountId { get; private set; }
        
        public DateTime TradeDate { get; private set; }
        
        public string Status { get; private set; }

        public Trade(string tickerSymbol, int quantity, decimal price, Guid buyerAccountId, Guid sellerAccountId)
        {
            if (quantity <= 0) throw new ArgumentException("Quantidade deve ser maior que zero."); 
            if (price <= 0) throw new ArgumentException("Preço deve ser maior que zero.");

            Id = Guid.NewGuid(); 
            TickerSymbol = tickerSymbol; 
            Quantity = quantity;
            Price = price;
            BuyerAccountId = buyerAccountId;
            SellerAccountId = sellerAccountId;
            TradeDate = DateTime.UtcNow;
            Status = "Pending";
        }

        public decimal CalculateTotalValue() 
        {
            return Quantity * Price; 
        }

        public void MarkAsSettled()
        {
            Status = "Settled";
        }

        public void MarkAsFailed()
        {
            Status = "Failed";
        }
    }
}