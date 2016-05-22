using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bets.Data;
using Bets.Data.Models;

namespace Bets.Controllers
{
    public class MatchBetController : CRUDController<MatchBetModel, MatchBet, MatchBetRepository>
    {
		public MatchBetController()
		{
			this.Name = "MatchBet";
			this.Module = Module.Dashboard;

			this.ListingViewModel.IsSubPage = true;
			this.ListingViewModel.MinPageSize = 20;
		}
    }
}
