using System;
using System.Web.Mvc;
using Bets.Data;
using Bets.Data.Models;

namespace Bets.Controllers
{
    public class WinnerResultController : CRUDController<TeamModel, Team, WinnerResultRepository>
    {
        public WinnerResultController()
        {
            this.Name = "WinnerResult";
            this.Module = Module.MyBets;

            this.ListingViewModel.HasSearch = false;
            this.ListingViewModel.IsSubPage = true;
            this.ListingViewModel.MinPageSize = 4;
            this.ListingViewModel.TemplateName = "Horizontal";
        }

        protected override void SetControllerData()
        {
            base.SetControllerData();
            ViewBag.Expired = true;
        }

        protected override void SetDefaultListingData(TeamModel model)
        {
            base.SetDefaultListingData(model);
            model.BetExpired = true;
        }

        public override ActionResult Index(int? id, string pageType, bool? isLookup, TeamModel model = null, ListingParams<TeamModel> listingParams = null)
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