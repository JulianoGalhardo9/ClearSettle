import { useEffect, useState } from 'react';
import { api } from './services/api';
import type { Trade } from './types/Trade';
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr'; 

function App() {
  const [trades, setTrades] = useState<Trade[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchTrades();

    const connection = new HubConnectionBuilder()
      .withUrl('http://localhost:5000/tradeHub') 
      .configureLogging(LogLevel.Information)
      .withAutomaticReconnect() 
      .build();

    connection.start()
      .then(() => console.log('🟢 Conectado ao túnel WebSocket do SignalR!'))
      .catch(err => console.error('🔴 Erro ao conectar no SignalR: ', err));

    connection.on('ReceiveTradeUpdate', (messageJson: string) => {
      const updatedTrade: Trade = JSON.parse(messageJson);
      
      setTrades(currentTrades => 
        currentTrades.map(trade => 
          trade.id === updatedTrade.id ? updatedTrade : trade
        )
      );
    });

    return () => {
      connection.stop();
    };
  }, []);

  const fetchTrades = async () => {
    try {
      const response = await api.get('/Trades');
      setTrades(response.data);
    } catch (error) {
      console.error("Erro ao buscar operações:", error);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="container mx-auto p-8">
      <div className="flex justify-between items-center mb-6">
        <div>
          <h1 className="text-3xl font-bold text-blue-400">ClearSettle Backoffice</h1>
          <p className="text-gray-400 mt-2">Painel de Conciliação D+2 (Real-Time)</p>
        </div>
        {/* O botão ainda fica aqui por garantia, mas quase não precisaremos dele! */}
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
                <th className="p-4 text-sm font-semibold text-gray-300">Data</th>
                <th className="p-4 text-sm font-semibold text-gray-300">Status</th>
              </tr>
            </thead>
            <tbody>
              {trades.length === 0 ? (
                <tr>
                  <td colSpan={6} className="p-4 text-center text-gray-500">Nenhuma operação encontrada.</td>
                </tr>
              ) : (
                trades.map((trade) => (
                  <tr key={trade.id} className="border-b border-gray-700 hover:bg-gray-750 transition-all duration-500">
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
                      <span className={`px-2 py-1 rounded text-xs font-semibold transition-colors duration-500 ${
                        trade.status === 'Settled' ? 'bg-green-900 text-green-300 shadow-[0_0_10px_rgba(34,197,94,0.3)]' : 
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