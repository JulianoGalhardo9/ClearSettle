import { useEffect, useState } from 'react';
import { api } from './services/api';
import type { Trade } from './types/Trade';

function App() {
  const [trades, setTrades] = useState<Trade[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchTrades();
  }, []);

  const fetchTrades = async () => {
    try {
      const response = await api.get('/Trades');
      setTrades(response.data);
    } catch (error) {
      console.error("Erro ao buscar operações do backend:", error);
    } finally {
      setLoading(false); 
    }
  };

  return (
    <div className="container mx-auto p-8">
      <div className="flex justify-between items-center mb-6">
        <div>
          <h1 className="text-3xl font-bold text-blue-400">ClearSettle Backoffice</h1>
          <p className="text-gray-400 mt-2">Painel de Conciliação e Liquidação D+2</p>
        </div>
        <button 
          onClick={fetchTrades}
          className="bg-blue-600 hover:bg-blue-500 text-white px-4 py-2 rounded-md font-medium transition-colors"
        >
          Atualizar Dados
        </button>
      </div>

      {loading ? (
        <p className="text-gray-400 animate-pulse">Carregando operações...</p>
      ) : (
        <div className="bg-gray-800 rounded-lg shadow-lg overflow-hidden border border-gray-700">
          <table className="w-full text-left border-collapse">
            <thead>
              <tr className="bg-gray-900 border-b border-gray-700">
                <th className="p-4 text-sm font-semibold text-gray-300">ID</th>
                <th className="p-4 text-sm font-semibold text-gray-300">Ativo</th>
                <th className="p-4 text-sm font-semibold text-gray-300">Qtd</th>
                <th className="p-4 text-sm font-semibold text-gray-300">Preço</th>
                <th className="p-4 text-sm font-semibold text-gray-300">Data da Operação</th>
                <th className="p-4 text-sm font-semibold text-gray-300">Status</th>
              </tr>
            </thead>
            <tbody>
              {trades.length === 0 ? (
                <tr>
                  <td colSpan={6} className="p-4 text-center text-gray-500">Nenhuma operação encontrada no banco de dados.</td>
                </tr>
              ) : (
                trades.map((trade) => (
                  <tr key={trade.id} className="border-b border-gray-700 hover:bg-gray-750 transition-colors">
                    <td className="p-4 text-sm text-gray-400 font-mono">{trade.id.substring(0, 8)}...</td>
                    <td className="p-4 text-sm font-bold text-white">{trade.tickerSymbol}</td>
                    <td className="p-4 text-sm text-gray-300">{trade.quantity}</td>
                    <td className="p-4 text-sm text-gray-300">
                      {new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(trade.price)}
                    </td>
                    <td className="p-4 text-sm text-gray-400">
                      {new Date(trade.tradeDate).toLocaleString('pt-BR')}
                    </td>
                    <td className="p-4 text-sm">
                      <span className={`px-2 py-1 rounded text-xs font-semibold ${
                        trade.status === 'Settled' ? 'bg-green-900 text-green-300' : 
                        trade.status === 'Pending' ? 'bg-yellow-900 text-yellow-300' : 
                        'bg-red-900 text-red-300'
                      }`}>
                        {trade.status}
                      </span>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}

export default App;