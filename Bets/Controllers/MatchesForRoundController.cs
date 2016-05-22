using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bets.Data;
using Bets.Data.Models;

namespace Bets.Controllers
{
	public class MatchesForRoundController : CRUDController<MatchForRoundModel, Match, MatchesForRoundRepository>
	{
		public MatchesForRoundController()
		{
			this.Name = "MatchesForRound";
			this.Module = Module.RoundBets;

			this.ListingViewModel.IsSubPage = true;
			this.ListingViewModel.MinPageSize = 16;
		}

		protected override void SetListingData(MatchForRoundModel model)
		{
			base.SetListingData(model);

			this.ListingViewModel.Parameters = new Dictionary<string,object> { { "RoundID", model.RoundID } };
		}
	}
}
