using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net;
using System.Configuration;

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
			var message = new MailMessage();
			var smtpClient = new SmtpClient();

			message.Sender = message.From;
			message.To.Add(new MailAddress(email.To, email.ToName));
			message.IsBodyHtml = true;
			message.Subject = email.Subject;
			message.Body = email.Body;

			try
			{
				smtpClient.Send(message);
				message.Dispose();
				smtpClient.Dispose();

				return true;
			}
			catch (Exception ex)
			{
                Logger.Log(ex);

                if (!handleError)
                {
                    throw;
                }

                return false;
			}
		}
	}
}
