using System.Net;
using Newtonsoft.Json.Linq;

namespace StockPriceNotifier
{
    public class NasdaqStockDataProvider : IStockDataProvider
    {
        public StockData GetLatestTickerPrice(string ticker)
        {
            try
            {
                using (var web = new WebClient())
                {
                    var url = $"http://finance.google.com/finance/info?client=ig&q=NASDAQ%3A{ticker}";
                    var json = web.DownloadString(url);
                    json = json.Replace("//", "");

                    return new StockData
                    {
                        Ticker = ticker,
                        LastPrice = (decimal)JArray.Parse(json)[0].SelectToken("l")
                    };
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
