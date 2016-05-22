using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bets.Data.Models;
using Bets.Data;

namespace Bets.Controllers
{
	public class GoalscorerStandingsController : CRUDController<GoalscorerModel, Player, GoalscorerStandingsRepository>
	{
		private bool expired = new MatchRepository().GlobalBetsExpired();

		public GoalscorerStandingsController()
		{
			this.Name = "GoalscorerStandings";
			this.Module = Module.Dashboard;

			this.ListingViewModel.IsSubPage = true;
			this.ListingViewModel.MinPageSize = 6;
			this.ListingViewModel.TemplateName = "Horizontal";
		}

		protected override void SetControllerData()
		{
			base.SetControllerData();
			ViewBag.Expired = expired;
		}

		protected override void SetDefaultListingData(GoalscorerModel model)
		{
			base.SetDefaultListingData(model);
			model.BetExpired = expired;
		}

		protected override void SetListingData(GoalscorerModel model)
		{
			base.SetListingData(model);
			ViewBag.AllowBet = false;
		}
	}
}
