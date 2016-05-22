using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ThatAuthentication;
using Bets.Data.Models;
using Bets.Data;
using System.Web.Security;

namespace Bets.Controllers
{
	public class MyBetsController : BaseController
	{
		public MyBetsController() : base(Module.MyBets) 
		{
			var matchRepo = new MatchRepository();

			ViewBag.RoundExpired = matchRepo.CurrentRoundIsExpired();
			ViewBag.Expired = matchRepo.GlobalBetsExpired();
		}

		protected override void SetIndexData()
		{
			base.SetIndexData();

			var userID = Convert.ToInt32(Membership.GetUser().ProviderUserKey);
			var goalscorerRepo = new GoalscorerForRoundRepository(userID);

			ViewBag.RoundGoalscorerBet = goalscorerRepo.GetGoalscorerBetForCurrentRound();
			ViewBag.GlobalGoalscorerBet = goalscorerRepo.GetGlobalGoalscorerBet();
			ViewBag.WinnerBet = new WinnerRepository(userID).GetWinnerBet();
			ViewBag.MaxBonusPerMatch = new SettingsRepository().GetItem(0).MaxBonusPerMatch;
		}

		public ActionResult GetGoalscorerForRoundBet()
		{
			var userID = Convert.ToInt32(Membership.GetUser().ProviderUserKey);
			var repo = new GoalscorerForRoundRepository(userID);

			return PartialView("Goalscorer", repo.GetGoalscorerBetForCurrentRound());
		}

		public ActionResult GetGoalscorerBet()
		{
			var userID = Convert.ToInt32(Membership.GetUser().ProviderUserKey);
			var repo = new GoalscorerForRoundRepository(userID);

			return PartialView("Goalscorer", repo.GetGlobalGoalscorerBet());
		}

		public ActionResult GetWinnerBet()
		{
			var userID = Convert.ToInt32(Membership.GetUser().ProviderUserKey);
			var repo = new WinnerRepository(userID);

			return PartialView("Winner", repo.GetWinnerBet());
		}
	}
}
