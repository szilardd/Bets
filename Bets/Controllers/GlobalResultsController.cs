using System;
using System.Web.Mvc;
using Bets.Data.Models;

namespace Bets.Controllers
{
    public class GlobalResultsController : BaseController
    {
        public GlobalResultsController()
        {
            this.name = "GlobalResults";
            this.Module = Module.GlobalResults;
        }

        public override ActionResult Index()
        {
            if (!TournamentEnded)
            {
                return Content("Not yet");
            }
            else
            {
                return base.Index();
            }
        }
    }
}