using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bets.Data;
using Bets.Data.Models;

namespace Bets.Controllers
{
    public class HelpController : BaseController
	{
		public HelpController() : base(Module.Help)
        {
            ViewBag.MaxBonusPerMatch = new SettingsRepository().GetItem(0).MaxBonusPerMatch;
        }
	}
}
