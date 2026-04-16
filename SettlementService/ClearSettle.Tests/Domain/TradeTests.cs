using ClearSettle.Domain.Entities;
using FluentAssertions;

namespace ClearSettle.Tests.Domain
{
    public class TradeTests
    {
        [Fact]
        public void CreateTrade_WithValidParameters_ShouldInitializeCorrectly()
        {
            // Arrange (Preparação)
            var ticker = "PETR4";
            var quantity = 100;
            var price = 30.5m;
            var buyer = Guid.NewGuid();
            var seller = Guid.NewGuid();

            // Act (Ação)
            var trade = new Trade(ticker, quantity, price, buyer, seller);

            // Assert (Verificação)
            trade.Status.Should().Be("Pending");
            trade.TickerSymbol.Should().Be(ticker);
            trade.CalculateTotalValue().Should().Be(3050m);
        }

        [Theory] 
        [InlineData(0)]
        [InlineData(-10)]
        public void CreateTrade_WithInvalidQuantity_ShouldThrowException(int invalidQuantity)
        {
            // Arrange
            var price = 10m;

            // Act
            Action act = () => new Trade("VALE3", invalidQuantity, price, Guid.NewGuid(), Guid.NewGuid());

            // Assert
            act.Should().Throw<ArgumentException>()
               .WithMessage("Quantidade deve ser maior que zero.");
        }
    }
}