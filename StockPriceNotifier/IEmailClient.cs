using System.Net.Mail;

namespace StockPriceNotifier
{
    public interface IEmailClient
    {
        void SendEmail(MailMessage m);
    }

}
