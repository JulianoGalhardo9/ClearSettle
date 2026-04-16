namespace ClearSettle.Application.DTOs
{
    public record RegisterTradeInput(
        string TickerSymbol,
        int Quantity,
        decimal Price,
        Guid BuyerAccountId,
        Guid SellerAccountId
    );
}