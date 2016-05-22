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
    public class GoalscorerForUserBetsController : Controller
    {
        //
        // GET: /GoalscorerForUserBets/
        private bool expired = new MatchRepository().GlobalBetsExpired();

        public ActionResult Index(int userID)
        {
            var goalscorers = new GoalscorerRepository().GetGoalScorerForUserBets(userID);
            var goalscorer = new GoalscorerModel();
            ViewBag.Count = 0;

            if (goalscorers.Count() > 0)
            {
                goalscorer = goalscorers.First();
                ViewBag.Count = 1;
            }

            ViewBag.Expired = DataExtensions.UserIsAdmin() || expired;

            return View(goalscorer);
        }

    }
}
