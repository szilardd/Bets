using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bets.ViewModels
{
    public class DashboardViewModel
    {
		public IQueryable<Data.Models.MatchModel> Matches { get; set; }
	}
}
