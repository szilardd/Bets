using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bets.Data.Models;
using Bets.Data;

namespace Bets.Controllers
{
	public class WinnerController : CRUDController<TeamModel, Team, WinnerRepository>
	{
		private bool expired = new MatchRepository().GlobalBetsExpired();

		public WinnerController()
		{
			this.Name = "Winner";
			this.Module = Module.MyBets;

			this.ListingViewModel.HasSearch = !expired;
			this.ListingViewModel.IsSubPage = true;
			this.ListingViewModel.MinPageSize = 4;
			this.ListingViewModel.TemplateName = "Horizontal";
		}

		protected override void SetControllerData()
		{
			base.SetControllerData();

			ViewBag.Expired = expired;
		}

		protected override void SetDefaultListingData(TeamModel model)
		{
			base.SetDefaultListingData(model);

			model.BetExpired = expired;
		}
	}
}
