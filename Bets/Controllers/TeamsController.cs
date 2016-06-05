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
    /// <summary>
    /// Cache for 10 minutes
    /// </summary>
    [OutputCache(Duration = 600, VaryByParam = "none")]
    public partial class TeamsController : BaseController
	{
		public TeamsController() : base(Module.Teams) 
        {
            this.name = "Teams";
        }

        protected override void SetIndexData()
        {
            base.SetIndexData();
			ViewData.Model = new TeamRepository().GetAllGroupStandings();
        }
	}
}
