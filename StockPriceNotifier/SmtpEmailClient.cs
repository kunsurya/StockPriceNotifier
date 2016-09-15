using System.Net;
using System.Net.Mail;

namespace StockPriceNotifier
{
    public class SmtpEmailClient : IEmailClient
    {
        public void SendEmail(MailMessage m)
        {
            using (var client = new SmtpClient
            {
                Port = 25,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Host = "smtp.gmail.com",
                EnableSsl = true,
                Credentials = new NetworkCredential(m.From.Address, "qliluscygaqxcjhax")
            })
            {
                client.Send(m);
            }
        }
    }
}
