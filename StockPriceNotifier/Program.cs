namespace StockPriceNotifier
{
    class Program
    {
        static void Main(string[] args)
        {
            new StockNotifier(new NasdaqStockDataProvider(), new SmtpEmailClient(), "GOOG", 760, PriceDirection.Down)
                                    .Register("kunal.suryavanshi@gmail.com");
        }
    }
}
