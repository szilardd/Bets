using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bets.Data;
using Bets.Data.Models;
using Bets.Helpers;

namespace Bets.Controllers
{
	public class MatchesForCurrentRoundController : CRUDController<MatchForRoundModel, Match, MatchesForRoundRepository>
    {
		public MatchesForCurrentRoundController()
		{
			this.Name = "MatchesForCurrentRound";
			this.Module = Module.Dashboard;

			this.ListingViewModel.IsSubPage = true;
			this.ListingViewModel.MinPageSize = 20;

            ViewBag.MaxBonusPerMatch = new SettingsRepository().GetItem(0).MaxBonusPerMatch;
		}

		private Dictionary<string, object> GetUserData()
		{
			return new Dictionary<string, object> { { "UserBonus", this.repo.GetUserBonus() } };
		}

		protected override void SetIndexData(MatchForRoundModel model)
		{
			this.ListingViewModel.Parameters = this.GetUserData();
		}

		protected override void SetListingData(MatchForRoundModel model)
		{
			base.SetListingData(model);
            model.Current = true;
		}

		protected override void SetDefaultListingData(MatchForRoundModel model)
		{
			base.SetDefaultListingData(model);
            model.Current = true;
		}

		protected override Dictionary<string, object> AfterSave(DBActionType actionType, MatchForRoundModel model)
		{
			return this.GetUserData();
		}
    }
}
