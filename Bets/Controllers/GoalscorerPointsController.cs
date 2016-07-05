using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bets.Data;
using Bets.Data.Models;

namespace Bets.Controllers
{
    public class GoalscorerPointsController : CRUDController<GoalscorerPointsModel, Team, GoalscorerPointsRepository>
    {
        public GoalscorerPointsController()
        {
            this.Name = "GoalscorerPoints";
            this.Module = Module.MyBets;

            this.ListingViewModel.HasSearch = false;
            this.ListingViewModel.IsSubPage = true;
            this.ListingViewModel.MinPageSize = 50;
        }

        public override ActionResult Index(int? id, string pageType, bool? isLookup, GoalscorerPointsModel model = null, ListingParams<GoalscorerPointsModel> listingParams = null)
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