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
		private MatchesForRoundRepository matchRepo = new MatchesForRoundRepository();
		
		public NotificationMailer(): base()
		{
			MasterName="_Layout";
		}

		public MailMessage Match()
		{
			return null;
		}
		
		public void RoundNotification(int roundID)
		{
            var mailMessage = new MailMessage();
            var userRepo = new UserRepository(this.matchRepo.Context, matchRepo.UserID);
			var userEmails = userRepo.GetActiveUsers().Select(e => new { e.Email, e.UserID }).ToList();	
			var listingDataModel = new ListingParams<MatchForRoundModel> { Model = new MatchForRoundModel { RoundID = roundID, ForNotification = true } };

            var matchForRoundRepo = new MatchesForRoundRepository(userRepo.Context);

            ViewData.Model = matchRepo.GetMatchesForRound(listingDataModel).ToList();

            //send notification to all users
            foreach (var user in userEmails)
			{
                if (!string.IsNullOrEmpty(user.Email))
                {
                    matchForRoundRepo.UserID = user.UserID;
                    
                    ViewBag.BonusPointsLeft = matchForRoundRepo.GetUserBonus();

                    PopulateBody(mailMessage, viewName: "RoundNotification");

                    Email.Send(new Email
                    {
                        Body = mailMessage.Body,
                        Subject = "Notification for current round",
                        To = user.Email
                    });
                }
			}
		}

		public void MatchNotificationForToday()
		{
            var mailMessage = new MailMessage();
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