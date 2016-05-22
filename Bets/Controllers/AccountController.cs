using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Bets.Data.Models;

namespace Bets.Controllers
{
	public partial class AccountController : Controller
	{
		// GET: /Account/LogOn
		public virtual ActionResult LogOn()
		{
			if (TempData["AccessDenied"] != null)
				ViewBag.Message = "You don't have permission to perform the previous action. Please log in with a different user.";

			return PartialView();
		}

		// POST: /Account/LogOn
		[HttpPost]
		public virtual ActionResult LogOn(LogOnModel model, string returnUrl)
		{
			if (ModelState.IsValid)
			{
				if (Membership.ValidateUser(model.UserName, model.Password))
				{
					FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);

					if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
						&& !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
					{
						return Redirect(returnUrl);
					}
					else
					{
						return RedirectToAction("Index", "Dashboard");
					}
				}
				else
				{
					ModelState.AddModelError("", "The user name or password provided is incorrect.");
				}
			}

			// If we got this far, something failed, redisplay form
			return PartialView(model);
		}

		// GET: /Account/LogOff
		public virtual ActionResult LogOff()
		{
			FormsAuthentication.SignOut();
			Session.Abandon();

			return RedirectToAction("Index", "Dashboard");
		}
	}
}
