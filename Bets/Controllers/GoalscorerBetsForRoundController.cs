using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bets.Data;
using Bets.Data.Models;

namespace Bets.Controllers
{
	public class GoalscorerBetsForRoundController : CRUDController<GoalscorerBetModel, Player, GoalscorerBetsForRoundRepository>
	{
		public GoalscorerBetsForRoundController()
		{
			this.Name = "GoalscorerBetsForRound";
			this.Module = Module.RoundBets;

			this.ListingViewModel.IsSubPage = true;
			this.ListingViewModel.MinPageSize = 5;
		}

		protected override void SetListingData(GoalscorerBetModel model)
		{
			base.SetListingData(model);

			this.ListingViewModel.Parameters = new Dictionary<string, object> { { "RoundID", model.RoundID } };
		}
	}
}
