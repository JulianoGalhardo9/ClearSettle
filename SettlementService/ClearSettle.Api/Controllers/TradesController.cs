using System;
using System.Threading.Tasks;
using ClearSettle.Application.DTOs;
using ClearSettle.Infrastructure.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace ClearSettle.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TradesController : ControllerBase
    {
        private readonly RabbitMqPublisher _messagePublisher;

        public TradesController(RabbitMqPublisher messagePublisher)
        {
            _messagePublisher = messagePublisher;
        }

        [HttpPost]
        public async Task<IActionResult> EnqueueTrade([FromBody] RegisterTradeInput input)
        {
            try
            {
                await _messagePublisher.PublishAsync(input, "trade_pending_queue");

                return Accepted("", new { Message = "Ordem recebida e enviada para a fila de processamento." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "Erro ao enfileirar a operação.", Details = ex.Message });
            }
        }
    }
}