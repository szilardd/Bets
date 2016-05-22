using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ThatAuthentication;
using Bets.Data.Models;
using Bets.ViewModels;
using Bets.Data;

namespace Bets.Controllers
{
	public partial class DashboardController : BaseController
	{
		public DashboardController() : base(Module.Dashboard) { }

		protected override void SetIndexData()
		{
			base.SetIndexData();

			ViewBag.MaxBonusPerMatch = new SettingsRepository().GetItem(0).MaxBonusPerMatch;
		}

        public ViewResult Standings()
        {
            ViewBag.Module = Module.DashboardStandings;
            return View("DashboardStandings");
        }

        public ViewResult GoalscorerStandings()
        {
            ViewBag.Module = Module.DashboardGoalscorerStandings;
            return View("DashboardGoalscorerStandings");
        }
	}
}
