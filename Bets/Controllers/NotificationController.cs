using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bets.Mailers;
using Bets.Data;
using Bets.Infrastructure;

namespace Bets.Controllers
{
    public class NotificationController : Controller
    {
        public ActionResult Index()
        {
			var settingRepo = new SettingsRepository();
			var setting = settingRepo.GetItem(0);
			var sendRoundNotification = (setting.LastNotificationRoundID != setting.CurrentRoundID);
			
			try
			{
				var mailMessage = new NotificationMailer();

				//send round notifications only once
				if (sendRoundNotification)
				{
					//get first match date in round
					var firstDate = settingRepo.Context.Matches
								.Where(e => e.RoundID == setting.CurrentRoundID)
								.Select(e => e.Date).Min();

					//send notification only if the first match is today
					if (firstDate.Date == DateTime.UtcNow.Date)
					{
						mailMessage.RoundNotification(setting.CurrentRoundID);
						settingRepo.SetLastNotificationRoundID();
					}
				}
				//otherwise send match bet reminders for today
				else
				{
					mailMessage.MatchNotificationForToday();
				}
			}
			catch (Exception ex)
			{
                Logger.Log(ex);
			}

			return null;
        }
    }
}
