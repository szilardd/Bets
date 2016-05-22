using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bets.Data;
using Bets.Data.Models;

namespace Bets.Controllers
{
    public class BestUsersForRoundController : CRUDController<UserModel, User, BestUsersForRoundRepository>
    {
		public BestUsersForRoundController()
		{
			this.Name = "BestUsersForRound";
			this.Module = Module.Dashboard;

			this.ListingViewModel.IsSubPage = true;
			this.ListingViewModel.MinPageSize = 5;
			this.ListingViewModel.ItemTemplateName = "RoundStandings";
		}

    }
}
