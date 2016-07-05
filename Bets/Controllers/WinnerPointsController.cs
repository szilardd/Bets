using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bets.Data;
using Bets.Data.Models;

namespace Bets.Controllers
{
    public class WinnerPointsController : CRUDController<WinnerPointsModel, Team, WinnerPointsRepository>
    {
        private readonly bool _tournamentEnded;

        public WinnerPointsController()
        {
            this.Name = "WinnerPoints";
            this.Module = Module.MyBets;

            this.ListingViewModel.HasSearch = false;
            this.ListingViewModel.IsSubPage = true;
            this.ListingViewModel.MinPageSize = 50;

            var settingsRepo = new SettingsRepository();

            _tournamentEnded = settingsRepo.GetTournamentEndStatus();
        }

        public override ActionResult Index(int? id, string pageType, bool? isLookup, WinnerPointsModel model = null, ListingParams<WinnerPointsModel> listingParams = null)
        {
            if (!TournamentEnded)
            {
                return Content("Not yet");
            }
            else
            {
                return base.Index(id, pageType, isLookup, model, listingParams);
            }
        }
    }
}