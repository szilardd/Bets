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
    public class WinnerForUserBetsController : Controller
    {
        //
        // GET: /GoalscorerForUserBets/
        private bool expired = new MatchRepository().GlobalBetsExpired();

        public ActionResult Index(int userID)
        {
            var winners = new WinnerRepository().GetWinnerForUserBets(userID);
            var winner = new TeamModel();
            ViewBag.Count = 0;
            if (winners.Count() > 0)
            {
                winner = winners.First();
                ViewBag.Count = 1;
            }
			ViewBag.Expired = DataExtensions.UserIsAdmin() || expired;

            return View(winner);
        }

    }
}

