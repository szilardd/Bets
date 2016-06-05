using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bets.Data;
using Bets.Data.Models;

namespace Bets.Controllers
{
    /// <summary>
    /// Cache for 10 minutes
    /// </summary>
    [OutputCache(Duration = 600, VaryByParam = "none")]
    public class HelpController : BaseController
	{
		public HelpController() : base(Module.Help)
        {
            ViewBag.MaxBonusPerMatch = new SettingsRepository().GetItem(0).MaxBonusPerMatch;
        }
	}
}
