using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Reflection;
using Bets.Data;
using System.Web.Routing;
using System.IO;

namespace Bets.Helpers
{
	public static class UrlHelpers
	{
		private static string JavaScriptRoot = "~/content/js/";
		private static string CSSRoot = "~/content/css/";
		
		private static string PluginRoot = "~/content/plugins/";
		private static string TemplateRoot = "Templates/";
		private static string LayoutRoot = "Layouts/";
		private static string IncludeRoot = "Includes/";
		private static string FieldRoot = "Fields/";

		private static bool IsLiveMode()
		{
			return ConfigurationManager.AppSettings["LiveMode"] == "true";
		}

		private static string GetHash()
		{
			return ConfigurationManager.AppSettings["Version"];
		}

		public static string Script(this UrlHelper helper, string url)
		{
			return helper.Content(JavaScriptRoot + url + ".js");
		}

		public static string ScriptDev(this UrlHelper helper, string url)
		{
			return helper.Content(JavaScriptRoot + url + ".dev.js");
		}

		public static string ScriptPlugin(this UrlHelper helper, string url)
		{
			return helper.Content(PluginRoot + url + ".js");
		}

		public static string CSS(this UrlHelper helper, string url)
		{
			return helper.Content(CSSRoot + url + ".css");
		}

		public static string CSSPlugin(this UrlHelper helper, string url)
		{
			return helper.Content(PluginRoot + url + ".css");
		}

		public static string LESS(this UrlHelper helper, string url, bool custom = false)
		{
			return helper.Content(CSSRoot + url + ".less");
		}

		public static string CSSDev(this UrlHelper helper, string url)
		{
			return UrlHelpers.CSS(helper, url);
		}

		public static string Template(this UrlHelper helper, string name)
		{
			return TemplateRoot + "_" + name;
		}

		public static string Layout(this UrlHelper helper, string name, bool absolute = true)
		{
            if (absolute)
                return helper.Content("~/Views/Shared/" + LayoutRoot + "_" + name + "Layout.cshtml");
            else
                return LayoutRoot + "_" + name + "Layout";
		}

		public static string Include(this UrlHelper helper, string name)
		{
			return IncludeRoot + "_" + name + "Include";
		}

		public static string Field(this UrlHelper helper, string type, string name)
		{
			return FieldRoot + type + "/" + name + "Detail";
		}

		public static string ControllerAction(this UrlHelper helper, ControllerAction action, Dictionary<string, object> additionalData = null)
		{

			if (additionalData != null)
			{
				if (action.Data == null)
					action.Data = new RouteValueDictionary();

				foreach (var key in additionalData.Keys)
					action.Data[key] = additionalData[key];
			}

			return helper.Action(action.Action, action.Controller, action.Data);
		}
	}
}