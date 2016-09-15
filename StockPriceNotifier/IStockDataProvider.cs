namespace StockPriceNotifier
{
    public interface IStockDataProvider
    {
        StockData GetLatestTickerPrice(string ticker);
    }
}
