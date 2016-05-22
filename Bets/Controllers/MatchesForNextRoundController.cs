using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bets.Data;
using Bets.Data.Models;

namespace Bets.Controllers
{
	public class MatchesForNextRoundController : CRUDController<MatchForRoundModel, Match, MatchesForRoundRepository>
	{
		public MatchesForNextRoundController()
		{
			this.Name = "MatchesForNextRound";
			this.Module = Module.Dashboard;

			this.ListingViewModel.IsSubPage = true;
			this.ListingViewModel.MinPageSize = 5;
		}

		protected override void SetListingData(MatchForRoundModel model)
		{
			base.SetListingData(model);
			model.Current = false;
		}

		protected override void SetDefaultListingData(MatchForRoundModel model)
		{
			base.SetDefaultListingData(model);
			model.Current = false;
		}
	}
}
