using System; // Necessário para usar Guid
using System.Threading.Tasks; // Necessário para processamento assíncrono
using ClearSettle.Domain.Entities; // Importa a nossa entidade principal
using ClearSettle.Domain.Interfaces; // Importa o contrato (interface) que devemos cumprir
using Microsoft.EntityFrameworkCore; // Importa as ferramentas do Entity Framework Core

namespace ClearSettle.Infrastructure.Data.Repositories
{
    // A classe assina o contrato ITradeRepository, sendo obrigada a implementar seus métodos
    public class TradeRepository : ITradeRepository
    {
        private readonly SettlementDbContext _context; // Variável privada que guarda a sessão do banco de dados

        // Construtor que recebe a sessão do banco de dados por Injeção de Dependência
        public TradeRepository(SettlementDbContext context)
        {
            _context = context; // Salva o contexto para ser usado nos métodos abaixo
        }

        // Implementa a busca no banco de dados por ID
        public async Task<Trade?> GetByIdAsync(Guid id)
        {
            // O EF Core vai até a tabela Trades e procura a linha com esse ID
            return await _context.Trades.FindAsync(id);
        }

        // Implementa a inserção de uma nova operação no banco
        public async Task AddAsync(Trade trade)
        {
            // Adiciona a entidade na memória do EF Core (ainda não foi pro banco)
            await _context.Trades.AddAsync(trade);
            
            // Dispara o comando INSERT no banco de dados real
            await _context.SaveChangesAsync();
        }

        // Implementa a atualização de uma operação (ex: alterar status para Liquidado)
        public async Task UpdateAsync(Trade trade)
        {
            // O método Update não é assíncrono nativamente no EF Core, pois ele apenas marca a entidade como "Modificada" na memória
            _context.Trades.Update(trade);
            
            // Dispara o comando UPDATE no banco de dados real (este sim, precisa ser assíncrono)
            await _context.SaveChangesAsync();
        }
    }
}