using System.Net;
using System.Net.Mail;

namespace HangmanGameBusiness.Email
{
    public class SmtpEmailSender : IEmailSender
    {
        public void SendEmail(string to, string subject, string body)
        {
            EmailSettings settings = EmailSettingsReader.Read();

            MailMessage message = new MailMessage();

            message.From = new MailAddress(
                settings.SenderAddress,
                settings.SenderDisplayName
            );

            message.To.Add(to);
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = false;

            using (SmtpClient smtpClient = new SmtpClient(settings.SmtpHost, settings.SmtpPort))
            {
                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(
                    settings.SenderAddress,
                    settings.SenderPassword
                );

                smtpClient.Send(message);
            }
        }
    }
}
