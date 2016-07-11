using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bets.Data.Models;
using Bets.Models;
using System.Web.Security;
using Bets.Helpers;
using Bets.Data;

namespace Bets.Controllers
{
    public class BaseController : Controller
    {
		protected Module Module { get; set; }
        protected string name;
        private bool? _tournamentEnded;

        public BaseController()
		{
		}

		public BaseController(Module module)
		{
			this.Module = module;
		}

		protected virtual void SetData()
		{
			ViewBag.Module = this.Module;
		}

        protected bool TournamentEnded
        {
            get
            {
                if (_tournamentEnded == null)
                {
                    if (DataExtensions.UserIsAdmin())
                    {
                        _tournamentEnded = true;
                    }
                    else
                    {
                        var settingsRepo = new SettingsRepository();
                        _tournamentEnded = settingsRepo.GetTournamentEndStatus();
                    }
                }

                return _tournamentEnded.Value;
            }
        }

		protected virtual void SetIndexData()
		{
			this.SetData();

            ViewBag.Name = name;
			ViewBag.IsAdmin = false;
            ViewBag.TournamentEnded = TournamentEnded;

            if (Request.IsAuthenticated)
			{
				var user = Membership.GetUser() as ThatAuthentication.ThatMembershipUser;

				ViewBag.DisplayName = user.DisplayName;

				if (user != null)
					ViewBag.IsAdmin = (user.Role == Role.Admin);
			}
		}

		protected virtual void SetDetailData()
		{
			this.SetData();
		}

		public virtual ActionResult Index()
        {
			this.SetIndexData();
			return View(this.Module.ToString() + "Index", ViewData.Model);
        }
    }
}
