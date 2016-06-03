using System;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Bets.Data
{
    public class Email
    {
        public string Subject { get; set; }
        public string Body { get; set; }
        public string From { get; set; }
        public string FromName { get; set; }
        public string To { get; set; }
        public string ToName { get; set; }

        public static bool Send(Email email, bool handleError = true)
        {
            bool success = false;
            var smtpClient = new SmtpClient();
            var message = new MailMessage();

            message.Sender = new MailAddress(email.From, email.FromName);
            message.To.Add(new MailAddress(email.To, email.ToName));
            message.IsBodyHtml = true;
            message.Subject = email.Subject;
            message.Body = email.Body;

            try
            {
                smtpClient.Send(message);
                success = true;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                if (!handleError)
                {
                    throw;
                }
            }
            finally
            {
                if (message != null)
                {
                    message.Dispose();
                }

                if (smtpClient != null)
                {
                    smtpClient.Dispose();
                }
            }

            return success;
        }

        public static async Task<bool> SendAsync(Email email, bool handleError = true)
        {
            bool success = false;
            var smtpClient = new SmtpClient();
            var message = new MailMessage();

            message.Sender = new MailAddress(email.From, email.FromName);
            message.To.Add(new MailAddress(email.To, email.ToName));
            message.IsBodyHtml = true;
            message.Subject = email.Subject;
            message.Body = email.Body;

            try
            {
                await smtpClient.SendMailAsync(message);
                success = true;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                if (!handleError)
                {
                    throw;
                }
            }
            finally
            {
                if (message != null)
                {
                    message.Dispose();
                }

                if (smtpClient != null)
                {
                    smtpClient.Dispose();
                }
            }

            return success;
        }
    }
}
