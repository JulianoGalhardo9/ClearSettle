import { BarChart, Bar, XAxis, YAxis, Tooltip, ResponsiveContainer, CartesianGrid } from 'recharts';
import type { Trade } from '../types/Trade';

interface Props {
  trades: Trade[];
}

interface ChartData {
  ticker: string;
  volume: number;
}

export function VolumeChart({ trades }: Props) {
  const data = trades.reduce((acc: ChartData[], trade) => {
    const existing = acc.find(item => item.ticker === trade.tickerSymbol);
    const volume = trade.price * trade.quantity;

    if (existing) {
      existing.volume += volume;
    } else {
      acc.push({ ticker: trade.tickerSymbol, volume: volume });
    }
    return acc;
  }, []);

  return (
    <div className="bg-gray-800 p-6 rounded-lg border border-gray-700 h-80 mb-8 shadow-inner">
      <h2 className="text-xl font-semibold mb-4 text-blue-300">Volume Financeiro por Ativo (R$)</h2>
      <ResponsiveContainer width="100%" height="100%">
        <BarChart data={data}>
          <CartesianGrid strokeDasharray="3 3" stroke="#374151" vertical={false} />
          <XAxis dataKey="ticker" stroke="#9CA3AF" fontSize={12} tickLine={false} axisLine={false} />
          <YAxis stroke="#9CA3AF" fontSize={12} tickLine={false} axisLine={false} tickFormatter={(value) => `R$ ${value}`} />
          <Tooltip 
            cursor={{ fill: '#374151', opacity: 0.4 }}
            contentStyle={{ backgroundColor: '#111827', border: '1px solid #374151', borderRadius: '8px' }}
            itemStyle={{ color: '#60A5FA' }}
            formatter={(value) => {
              if (value === undefined || value === null) return ["R$ 0,00", "Volume Total"];
              
              const numericValue = Array.isArray(value) ? Number(value[0]) : Number(value);
              
              const formatted = new Intl.NumberFormat('pt-BR', { 
                style: 'currency', 
                currency: 'BRL' 
              }).format(numericValue);
              
              return [formatted, 'Volume Total'];
            }}
          />
          <Bar dataKey="volume" fill="#3B82F6" radius={[4, 4, 0, 0]} barSize={40} />
        </BarChart>
      </ResponsiveContainer>
    </div>
  );
}