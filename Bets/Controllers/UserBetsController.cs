using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ThatAuthentication;
using Bets.Data.Models;
using Bets.ViewModels;
using Bets.Data;
using System.Web.Security;
using Bets.Helpers;

namespace Bets.Controllers
{
	public partial class UserBetsController : BaseController
	{
		public UserBetsController() : base(Module.UserBets) 
        {
            this.name = "UserBets";
        }

        protected override void SetIndexData()
        {
            base.SetIndexData();
            var users = new UserRepository().GetActiveUsers().OrderByDescending(e => e.PointsWonBonus);
            ViewBag.Users = users.GetEnumerator();
			ViewBag.GlobalBetsExpired = new MatchRepository().GlobalBetsExpired();

            if (users.Count() > 0)
                ViewBag.FirstUserID = users.First().UserID;

			var rounds = new RoundRepository().GetFinishedRounds(DataExtensions.UserIsAdmin());
            ViewBag.Rounds = rounds.GetEnumerator();

            if (rounds.Count() > 0)
                ViewBag.FirstRoundID = rounds.First().ID;
        }
	}
}

