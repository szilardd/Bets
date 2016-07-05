using System;
using System.Web.Mvc;
using Bets.Data;
using Bets.Data.Models;

namespace Bets.Controllers
{
    public class GoalscorerResultController : CRUDController<GoalscorerModel, Player, GoalscorerResultRepository>
    {
        public GoalscorerResultController()
        {
            this.Name = "GoalscorerResult";
            this.Module = Module.MyBets;

            this.ListingViewModel.HasSearch = false;
            this.ListingViewModel.IsSubPage = true;
            this.ListingViewModel.MinPageSize = 3;
            this.ListingViewModel.TemplateName = "Horizontal";
        }

        protected override void SetControllerData()
        {
            base.SetControllerData();
            ViewBag.Expired = true;
        }

        protected override void SetDefaultListingData(GoalscorerModel model)
        {
            base.SetDefaultListingData(model);
            model.BetExpired = true;
            ViewBag.AllowBet = false;
        }

        public override ActionResult Index(int? id, string pageType, bool? isLookup, GoalscorerModel model = null, ListingParams<GoalscorerModel> listingParams = null)
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