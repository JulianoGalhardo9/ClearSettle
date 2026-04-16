using ClearSettle.Domain.Entities;
using ClearSettle.Domain.Interfaces;
using ClearSettle.Application.DTOs;

namespace ClearSettle.Application.UseCases
{
    public class RegisterPendingTradeUseCase
    {
        private readonly ITradeRepository _tradeRepository;

        public RegisterPendingTradeUseCase(ITradeRepository tradeRepository)
        {
            _tradeRepository = tradeRepository;
        }
        public async Task ExecuteAsync(RegisterTradeInput input)
        {
            var trade = new Trade(
                input.TickerSymbol, 
                input.Quantity, 
                input.Price, 
                input.BuyerAccountId, 
                input.SellerAccountId
            );

            await _tradeRepository.AddAsync(trade);
        }
    }
}