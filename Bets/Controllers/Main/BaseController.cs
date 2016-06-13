using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bets.Data.Models;
using Bets.Models;
using System.Web.Security;
using Bets.Helpers;

namespace Bets.Controllers
{
    public class BaseController : Controller
    {
		protected Module Module { get; set; }
        protected string name;

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

		protected virtual void SetIndexData()
		{
			this.SetData();

            ViewBag.Name = name;
			ViewBag.IsAdmin = false;
		
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

		public ActionResult Index()
        {
			this.SetIndexData();
			return View(this.Module.ToString() + "Index", ViewData.Model);
        }
    }
}
