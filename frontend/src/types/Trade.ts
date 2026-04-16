export interface Trade {
    id: string;
    tickerSymbol: string;
    quantity: number;
    price: number;
    status: string;
    tradeDate: string;
    buyerAccountId: string;
    sellerAccountId: string;
}