using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bets.Data;
using Bets.Data.Models;

namespace Bets.Controllers
{
    public class GoalscorerForRoundController : CRUDController<GoalscorerModel, Player, GoalscorerForRoundRepository>
    {
		public GoalscorerForRoundController()
		{
			this.Name = "GoalscorerForRound";
			this.Module = Module.Dashboard;

			this.ListingViewModel.HasSearch = true;
			this.ListingViewModel.IsSubPage = true;
			this.ListingViewModel.MinPageSize = 4;
			this.ListingViewModel.TemplateName = "Horizontal";
		}

		protected override void SetControllerData()
		{
			base.SetControllerData();

			ViewBag.Expired = new MatchRepository().CurrentRoundIsExpired();
		}
    }
}
