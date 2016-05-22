using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using Bets.Data;
using Bets.Data.Models;
using Mvc.Mailer;

namespace Bets.Mailers
{
    public class NotificationMailer : MailerBase, INotificationMailer     
	{
		private MailMessage mailMessage = new MailMessage();
		private MatchesForRoundRepository matchRepo = new MatchesForRoundRepository();
		
		public NotificationMailer(): base()
		{
			MasterName="_Layout";
		}

		public MailMessage Match()
		{
			return null;
		}
		
		public MailMessage RoundNotification(int roundID)
		{
			var userRepo = new UserRepository(this.matchRepo.Context, matchRepo.UserID);
			var userEmails = userRepo.GetActiveUsers().Select(e => e.Email).ToList();	
			var listingDataModel = new ListingParams<MatchForRoundModel> { Model = new MatchForRoundModel { RoundID = roundID, ForNotification = true } };

			ViewData.Model = matchRepo.GetMatchesForRound(listingDataModel).ToList();

			PopulateBody(mailMessage, viewName: "RoundNotification");

			//send notification to all users
			foreach (var email in userEmails)
			{
				Email.Send(new Email
				{
					Body = mailMessage.Body,
					Subject = "Notification for current round",
					To = email
				});
			}

			return mailMessage;
		}

		public void MatchNotificationForToday()
		{
			var matchesWithNoBets = matchRepo.GetMatchesWithNoBetsForToday();

			foreach (var email in matchesWithNoBets.Keys)
			{
				if (email != "" && matchesWithNoBets[email].Count != 0)
				{
					ViewData.Model = matchesWithNoBets[email];
					PopulateBody(mailMessage, viewName: "MatchNotificationForToday");

					Email.Send(new Email
					{
						Body = mailMessage.Body,
						Subject = String.Format("Notification for today's matches ({0})", DateTime.UtcNow.ToDate()),
						To = email
					});
				}
			}
		}
	}
}