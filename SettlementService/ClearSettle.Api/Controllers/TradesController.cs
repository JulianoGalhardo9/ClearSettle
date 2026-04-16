using ClearSettle.Application.DTOs;
using ClearSettle.Application.UseCases; 
using Microsoft.AspNetCore.Mvc;

namespace ClearSettle.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TradesController : ControllerBase
    {
        private readonly RegisterPendingTradeUseCase _registerTradeUseCase;

        public TradesController(RegisterPendingTradeUseCase registerTradeUseCase)
        {
            _registerTradeUseCase = registerTradeUseCase;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterTrade([FromBody] RegisterTradeInput input)
        {
            try
            {
                await _registerTradeUseCase.ExecuteAsync(input);

                return Created("", new { Message = "Operação registrada com sucesso e pendente de liquidação." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "Erro interno no servidor ao processar a operação.", Details = ex.Message });
            }
        }
    }
}