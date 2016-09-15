using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StockPriceNotifier
{
    public enum PriceDirection
    {
        Up = 0,
        Down = 1
    }

    public class StockEventArgs : EventArgs
    {
        public StockData StockData { get; }
        public StockEventArgs(StockData stockData)
        {
            StockData = stockData;
        }
    }

    public class StockNotifier
    {
        private readonly IStockDataProvider stockDataProvider;
        private readonly string stockSymbol;
        private readonly decimal notifyPrice;
        private readonly PriceDirection direction;
        private readonly IEmailClient emailClient;
        private event EventHandler<StockEventArgs> RaiseNotification;
        private string emailAddress;

        public StockNotifier(IStockDataProvider dataProvider, 
                             IEmailClient emailClient, 
                             string ticker, 
                             decimal thresholdPrice, 
                             PriceDirection direction)
        {
            stockDataProvider = dataProvider;
            stockSymbol = ticker;
            notifyPrice = thresholdPrice;
            this.direction = direction;
            this.emailClient = emailClient;
        }

        public StockNotifier(IStockDataProvider dataProvider,
                             string ticker,
                             decimal thresholdPrice,
                             PriceDirection direction)
        {
            stockDataProvider = dataProvider;
            stockSymbol = ticker;
            notifyPrice = thresholdPrice;
            this.direction = direction;
        }

        public void Register(string email)
        {
            RaiseNotification += StockPriceNotifier_RaiseNotification;
            emailAddress = email;
            //CheckStockPrice();
            try
            {
                Task.Factory.StartNew(CheckStockPrice).Wait();
            }
            catch (AggregateException)
            {
                    
                
            }
            
        }

        private void SendEmail(EmailData emailInfo)
        {
            try
            {
                var from = "kunal.suryavanshi@gmail.com";
                var mail = new MailMessage(from, emailInfo.To)
                {
                    Subject = emailInfo.Subject,
                    Body = emailInfo.Body,
                    BodyEncoding = Encoding.UTF8
                };

                //emailClient.SendEmail(mail);
                using (var client = new SmtpClient
                {
                    Port = 25,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Host = "smtp.gmail.com",
                    EnableSsl = true,
                    Credentials = new NetworkCredential(mail.From.Address, "qliluscygaqxcjhax")
                })
                {
                    client.Send(mail);
                }
            }
            catch { }
        }

        private void CheckStockPrice()
        {
            var stockData = stockDataProvider.GetLatestTickerPrice(stockSymbol);

            bool notify = false;

            do
            {

                notify = (direction == PriceDirection.Down)
                    ? stockData.LastPrice < notifyPrice
                    : stockData.LastPrice > notifyPrice;

                if (notify)
                {
                    RaiseNotification?.Invoke(this, new StockEventArgs(stockData));
                }
                else
                {
                    Thread.Sleep(600000);
                }
            } while (notify == false);
        }

        private void StockPriceNotifier_RaiseNotification(object sender, StockEventArgs e)
        {
            SendEmail(new EmailData
            {
                To = emailAddress,
                Subject = "Stock reached your target price!!",
                Body = $"Stock Ticker {e.StockData.Ticker} has current price of {e.StockData.LastPrice}"
            });
        }
    }
}
