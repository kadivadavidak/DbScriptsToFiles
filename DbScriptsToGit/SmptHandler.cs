using System.Net;
using System.Net.Mail;

namespace DbScriptsToGit
{
    class SmptHandler
    {
        // Create and send email.
        internal static void SendMessage(string subject, string body)
        {
            var email = new MailMessage("", "")
            {
                Subject = subject,
                Body = body
            };

            using (var client = new SmtpClient())
            {
                client.Port = 25;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Host = "";
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential("",
                    "");

                client.Send(email);
            }
        }
    }
}
