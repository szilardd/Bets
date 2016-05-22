using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bets.Data;
using Bets.Data.Models;

namespace Bets.Controllers
{
	public class WorstUsersForRoundController : CRUDController<UserModel, User, WorstUsersForRoundRepository>
	{
		public WorstUsersForRoundController()
		{
			this.Name = "WorstUsersForRound";
			this.Module = Module.Dashboard;

			this.ListingViewModel.IsSubPage = true;
			this.ListingViewModel.MinPageSize = 5;
			this.ListingViewModel.ItemTemplateName = "RoundStandings";
		}

	}
}
