using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bets.Data.Models;
using Bets.Data;

namespace Bets.Controllers
{
	public class StandingsController : CRUDController<UserModel, User, StandingsRepository>
	{
		public StandingsController()
		{
			this.Name = "Standings";
			this.Module = Module.Dashboard;

			this.ListingViewModel.IsSubPage = true;
			this.ListingViewModel.MinPageSize = 16;
			this.ListingViewModel.TemplateName = "Horizontal";
		}
	}
}
