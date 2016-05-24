using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.DynamicData;
using Bets.Controllers;
using Bets.Infrastructure;
using System.Web.WebPages;
using System.Text.RegularExpressions;
using System.Web.Http;
using Autofac;

namespace Bets
{
	public class MvcApplication : System.Web.HttpApplication
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			//all actions need authorization
			filters.Add(new LogonAuthorizeAttribute());
		}

		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("admin/{resource}.axd/{*pathInfo}");
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			MetaModel model = new MetaModel(); 
			model.RegisterContext(typeof(Bets.DynamicData.BetsEntities), new ContextConfiguration() { ScaffoldAllTables = true }); 
			routes.Add(new DynamicDataRoute("DD/{table}/{action}.aspx") { Constraints = new RouteValueDictionary(new { action = "List|Details|Edit|Insert" }), Model = model });

			routes.MapRoute(
				"Default", // Route name
				"{controller}/{action}/{id}", // URL with parameters
				new { controller = "Dashboard", action = "Index", id = UrlParameter.Optional } // Parameter defaults
			);
		}

        public static void RegisterModelBinders()
        {
            ModelBinders.Binders.DefaultBinder = new DateModelBinder();
        }

        #region "Application Events"

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            Server.ClearError();

            Response.Clear();

            var code = (exception is HttpException) ? (exception as HttpException).GetHttpCode() : 500;

            RouteData routeData = new RouteData();
            routeData.Values.Add("controller", "Error");
            routeData.Values.Add("action", (code == 404) ? "Http404" : "Index");
            routeData.Values.Add("error", exception);

            IController errorController = new ErrorController();
            errorController.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
        }

        protected void Application_Start()
		{
            RegisterRoutes(RouteTable.Routes);

            GlobalConfiguration.Configure(WebApiConfig.Register); // web api

            RegisterGlobalFilters(GlobalFilters.Filters);
			RegisterModelBinders();
            DisplayMode.Register();
            Bootstrapper.RegisterDependencyInjection();
        }

		protected void Application_EndRequest()
		{
			var context = new HttpContextWrapper(Context);

			//if an ajax request is made and the user and doing a 302, then the responses should be 401 (not authenticated)
			if (Context.Response.StatusCode == 302 && !Request.IsAuthenticated && context.Request.IsAjaxRequest())
			{
				Context.Response.Clear();
				Context.Response.StatusCode = 401;
			}
		}

		#endregion
	}
}