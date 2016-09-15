using System.Net.Mail.Fakes;
using System.Threading;
using System.Threading.Fakes;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using StockPriceNotifier;
using StockPriceNotifier.Fakes;

namespace StockPriceNotifierTests
{
    [TestFixture]
    [TestClass]
    public class StockPriceNotifierTests
    {
        //[Test]
        //public void NotifyTest_EmailSent_UsingMocks()
        //{
        //    Mockery mockery = new Mockery();
        //    IEmailClient emailClientMock = mockery.NewMock<IEmailClient>();
        //    IStockDataProvider stockDataProviderMock = mockery.NewMock<IStockDataProvider>();

        //    Expect.On(stockDataProviderMock)
        //        .Method("GetLatestTickerPrice")
        //        .WithAnyArguments()
        //        .Will(Return.Value(new StockData() {LastPrice = 760}));

        //    Expect.Once.On(emailClientMock).Method("SendEmail").WithAnyArguments();

        //    var stockNotifier = new StockNotifier(stockDataProviderMock, emailClientMock, "GOOG", 770, PriceDirection.Down);
        //    stockNotifier.Register("abc@gmail.com");
        //}

        //[TestMethod]
        //public void NotifyTest_EmailSent_UsingStubs()
        //{
        //    StubIStockDataProvider stockDataProviderStub = new StubIStockDataProvider()
        //    {
        //        GetLatestTickerPriceString = (s) => new StockData() {Ticker = s, LastPrice = 760}
        //    };

        //    bool messageSent = false;

        //    StubIEmailClient emailCLientStub = new StubIEmailClient()
        //    {
        //        SendEmailMailMessage = (m) => { messageSent = true; }
        //    };

        //    var stockNotifier = new StockNotifier(stockDataProviderStub, emailCLientStub, "GOOG", 770, PriceDirection.Down);
        //    stockNotifier.Register("dada@gmail.com");
        //    Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(messageSent);
        //}
        [TestMethod]
        public void NotifyTests_EmailSent_UsingShimsAndStubs()
        {
            StubIStockDataProvider stockDataProviderStub = new StubIStockDataProvider()
            {
                GetLatestTickerPriceString = (s) => new StockData() { Ticker = s, LastPrice = 760 }
            };

            using (ShimsContext.Create())
            {
                bool messageSent = false;
                ShimSmtpClient.AllInstances.SendMailMessage = (client, message) => { messageSent = true; };
                var stockNotifier = new StockNotifier(stockDataProviderStub, "GOOG", 770, PriceDirection.Down);
                stockNotifier.Register("dada@gmail.com");
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(messageSent);
            }
        }

        [TestMethod]
        public void NotifyTests_EmailNotSent_UsingShimsAndStubs()
        {
            StubIStockDataProvider stockDataProviderStub = new StubIStockDataProvider()
            {
                GetLatestTickerPriceString = (s) => new StockData() { Ticker = s, LastPrice = 780 }
            };

            using (ShimsContext.Create())
            {
                bool messageSent = false;
                ShimSmtpClient.AllInstances.SendMailMessage = (client, message) => { messageSent = true; };
                ShimThread.SleepInt32 = i => { Thread.CurrentThread.Abort(); };
                var stockNotifier = new StockNotifier(stockDataProviderStub, "GOOG", 770, PriceDirection.Down);
                stockNotifier.Register("dada@gmail.com");
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(messageSent);
            }
        }
    }
}
