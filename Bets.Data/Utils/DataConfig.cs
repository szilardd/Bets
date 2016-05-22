using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using System.Web;
using System.Configuration;

namespace Bets.Data
{
	public class ControllerAction
	{
		public string Controller { get; set; }
		public string Action { get; set; }

		//route data
		public RouteValueDictionary Data { get; set; }

		//unique identifier
		public object ID
		{
			get
			{
				if (this.Data == null || !this.Data.ContainsKey("id"))
					return null;

				return this.Data["id"];
			}

			set
			{
				if (this.Data == null)
					this.Data = new RouteValueDictionary();

				this.Data["id"] = value;
			}
		}
	}

	public class DataConfig
	{
		public static int ProfileImageWidth = 150;
		public static int ProfileImageHeight = 150;
		public static string DateFormat = "yyyy MMM dd";
		public static string DateTimeFormat = "yyyy MMM dd HH:mm";
		public static int MatchNotificationHour = 20;
		public static int HoursBeforeBet = 1;

		public static bool IsSuperUser()
		{
			bool isSuperAdmin = HttpContext.Current != null && HttpContext.Current.User != null && HttpContext.Current.User.Identity.Name == "admin";

			if (isSuperAdmin)
				return true;

			return false;
		}

		public static bool IsLiveMode()
		{
			return ConfigurationManager.AppSettings["LiveMode"] == "true";
		}

        public static bool EnableTracing()
        {
            return ConfigurationManager.AppSettings["EnableTracing"] == "true";
        }
	}
}
