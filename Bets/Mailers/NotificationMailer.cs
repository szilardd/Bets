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
            var userRepo = new UserRepository(this.matchRepo.Context, matchRepo.UserID);
            var users = userRepo.GetActiveUsers().Select(e => new UserModel { Email = e.Email, ID = e.UserID }).ToList();

            SendRoundNotificationToEmails(roundID, users, userRepo);   
		}

        public void RoundNotificationToAdmin(int roundID)
        {
            var userRepo = new UserRepository(this.matchRepo.Context, matchRepo.UserID);
            var adminUser = userRepo.Context.Users.Where(e => e.Username == "admin").Select(e => new UserModel { Email = e.Email, ID = e.UserID }).ToList();
            SendRoundNotificationToEmails(roundID, adminUser, userRepo);
        }

        private void SendRoundNotificationToEmails(int roundID, List<UserModel> users, UserRepository userRepo)
        {
            var mailMessage = new MailMessage();
            var listingDataModel = new ListingParams<MatchForRoundModel> { Model = new MatchForRoundModel { RoundID = roundID, ForNotification = true } };

            var matchForRoundRepo = new MatchesForRoundRepository(userRepo.Context);

            ViewData.Model = matchRepo.GetMatchesForRound(listingDataModel).ToList();

            //send notification to all users
            foreach (var user in users)
            {
                if (!string.IsNullOrEmpty(user.Email))
                {
                    matchForRoundRepo.UserID = user.ID;

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