using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bets.Data.Models;
using Bets.Data;

namespace Bets.Controllers
{
	public class StandingsBonusController : CRUDController<UserModel, User, StandingsBonusRepository>
	{
		public StandingsBonusController()
		{
			this.Name = "StandingsBonus";
			this.Module = Module.Dashboard;

			this.ListingViewModel.IsSubPage = true;
			this.ListingViewModel.MinPageSize = 20;
			this.ListingViewModel.TemplateName = "Horizontal";
		}
	}
}
