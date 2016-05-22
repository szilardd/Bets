using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ThatAuthentication;
using Bets.Data.Models;
using Bets.ViewModels;
using Bets.Data;

namespace Bets.Controllers
{
    public partial class RoundMatchesForUserController : CRUDController<MatchForRoundModel, Match, RoundMatchesForUserRepository>
	{
		public RoundMatchesForUserController()
		{
			this.Name = "RoundMatchesForUser";
			this.Module = Module.UserBets;
			this.ListingViewModel.IsSubPage = true;
			this.ListingViewModel.MinPageSize = 100;
		}
	}
}



