using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bets.Data.Models;
using Bets.Data;
using Bets.Helpers;

namespace Bets.Controllers
{
    public class RoundGoalscorerForUserController : Controller
    {
        //
        // GET: /GoalscorerForUserBets/

        public ActionResult Index(int userID, int roundID)
        {
            var expired = new RoundRepository().IsTheRoundExpired(roundID);
            var goalscorersforround = new GoalscorerRepository().GetRoundGoalScorerForUserBets(userID, roundID);
            var goalscorerforround = new GoalscorerModel();
            ViewBag.Count = 0;
            if (goalscorersforround.Count() > 0)
            {
                goalscorerforround = goalscorersforround.First();
                ViewBag.Count = 1;
            }
            ViewBag.RoundName = new RoundRepository().GetRoundName(roundID);
			ViewBag.Expired = DataExtensions.UserIsAdmin() || expired;
            return View(goalscorerforround);
        }

    }
}
