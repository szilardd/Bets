using System;
using System.Web.Mvc;
using System.Configuration;
using Bets.Data;

namespace Bets.Controllers
{
	public class ErrorController : Controller
	{
		// GET: /Error/
		public ActionResult Index(Exception error)
		{
			ViewBag.ShowError = !DataConfig.IsLiveMode();

			if (Request.IsAjaxRequest())
				return new HttpStatusCodeResult(500);
			else
				return View(error);
		}

		public ActionResult Http404()
		{
			return View();
		}
	}
}