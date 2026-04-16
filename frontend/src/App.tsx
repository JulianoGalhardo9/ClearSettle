import { useEffect, useState } from "react";
import { api } from "./services/api";
import type { Trade } from "./types/Trade";
import { HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import { VolumeChart } from "./components/VolumeChart"; // Importação correta

function App() {
  const [trades, setTrades] = useState<Trade[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchTrades();

    const connection = new HubConnectionBuilder()
      .withUrl("http://localhost:50/tradeHub")
      .configureLogging(LogLevel.Information)
      .withAutomaticReconnect()
      .build();

    connection
      .start()
      .then(() => console.log("🟢 SignalR Conectado"))
      .catch((err) => console.error("🔴 Erro SignalR: ", err));

    connection.on("ReceiveTradeUpdate", (messageJson: string) => {
      const updatedTrade: Trade = JSON.parse(messageJson);
      setTrades((currentTrades) =>
        currentTrades.map((trade) =>
          trade.id === updatedTrade.id ? updatedTrade : trade,
        ),
      );
    });

    return () => {
      connection.stop();
    };
  }, []);

  const fetchTrades = async () => {
    try {
      const response = await api.get("/Trades");
      setTrades(response.data);
    } catch (error) {
      console.error("Erro API:", error);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="container mx-auto p-8 max-w-6xl">
      <div className="flex justify-between items-center mb-8">
        <div>
          <h1 className="text-4xl font-extrabold text-blue-500 tracking-tight">
            ClearSettle{" "}
            <span className="text-white font-light">Backoffice</span>
          </h1>
          <p className="text-gray-400 mt-1">
            Monitoramento de liquidação em tempo real (D+2)
          </p>
        </div>
        <button
          onClick={fetchTrades}
          className="bg-blue-600 hover:bg-blue-500 text-white px-6 py-2 rounded-lg font-semibold transition-all shadow-lg active:scale-95"
        >
          Sincronizar
        </button>
      </div>

      {loading ? (
        <div className="flex justify-center items-center h-64">
          <p className="text-gray-500 text-xl animate-pulse">
            Conectando aos sistemas de custódia...
          </p>
        </div>
      ) : (
        <>
          {/* AQUI É ONDE O VOLUME CHART É USADO, RESOLVENDO O ERRO DE ESLINT */}
          <VolumeChart trades={trades} />

          <div className="bg-gray-800 rounded-xl shadow-2xl overflow-hidden border border-gray-700">
            <table className="w-full text-left">
              <thead>
                <tr className="bg-gray-900/50 border-b border-gray-700">
                  <th className="p-4 text-xs uppercase tracking-wider font-bold text-gray-500">
                    ID
                  </th>
                  <th className="p-4 text-xs uppercase tracking-wider font-bold text-gray-500">
                    Ativo
                  </th>
                  <th className="p-4 text-xs uppercase tracking-wider font-bold text-gray-500 text-right">
                    Qtd
                  </th>
                  <th className="p-4 text-xs uppercase tracking-wider font-bold text-gray-500 text-right">
                    Preço
                  </th>
                  <th className="p-4 text-xs uppercase tracking-wider font-bold text-gray-500">
                    Data
                  </th>
                  <th className="p-4 text-xs uppercase tracking-wider font-bold text-gray-500">
                    Status
                  </th>
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-700">
                {trades.length === 0 ? (
                  <tr>
                    <td
                      colSpan={6}
                      className="p-12 text-center text-gray-500 italic"
                    >
                      Aguardando entrada de novas ordens via RabbitMQ...
                    </td>
                  </tr>
                ) : (
                  trades.map((trade) => (
                    <tr
                      key={trade.id}
                      className={`hover:bg-gray-700/30 transition-all duration-700 ${
                        trade.status === "Settled" ? "animate-glow-green" : ""
                      }`}
                    >
                      <td className="p-4 text-sm text-gray-500 font-mono">
                        #{trade.id.substring(0, 5)}
                      </td>
                      <td className="p-4 text-sm font-bold text-white">
                        {trade.tickerSymbol}
                      </td>
                      <td className="p-4 text-sm text-gray-300 text-right">
                        {trade.quantity.toLocaleString()}
                      </td>
                      <td className="p-4 text-sm text-gray-300 text-right font-mono">
                        {new Intl.NumberFormat("pt-BR", {
                          style: "currency",
                          currency: "BRL",
                        }).format(trade.price)}
                      </td>
                      <td className="p-4 text-sm text-gray-400">
                        {new Date(trade.tradeDate).toLocaleTimeString("pt-BR")}
                      </td>
                      <td className="p-4">
                        <span
                          className={`px-3 py-1 rounded-full text-[10px] uppercase font-black tracking-widest transition-colors duration-500 ${
                            trade.status === "Settled"
                              ? "bg-green-500/10 text-green-400 border border-green-500/20 shadow-[0_0_15px_rgba(34,197,94,0.1)]"
                              : trade.status === "Pending"
                                ? "bg-yellow-500/10 text-yellow-400 border border-yellow-500/20"
                                : "bg-red-500/10 text-red-400 border border-red-500/20"
                          }`}
                        >
                          {trade.status}
                        </span>
                      </td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          </div>
        </>
      )}
    </div>
  );
}

export default App;
