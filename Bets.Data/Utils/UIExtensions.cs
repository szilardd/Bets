using System;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace Bets.Data
{
    public static class UIExtensions
    {
        private static string TeamFlagUrl = ConfigurationManager.AppSettings["TeamFlagUrl"];
        private static string TeamLogoUrl = ConfigurationManager.AppSettings["TeamLogoUrl"];
        private static string PlayerImageUrl = ConfigurationManager.AppSettings["PlayerImageUrl"];
        public const string ImageRoot = "~/content/img/";

        public static string ImgUrl(this UrlHelper helper, string url)
        {
            var baseUrl = helper.Action("", "", null, HttpContext.Current.Request.Url.Scheme);

            return baseUrl + ImageRoot.Replace("~/", "") + url;
        }

        public static string GetPlayerImage(this UrlHelper helper, string id)
        {
            return PlayerImageUrl + id + ".png";
        }

        public static string GetPlayerImage(string id)
        {
            return GetPlayerImage(null, id);
        }

        private static string AbsoluteUrl(UrlHelper helper)
        {
            // should be null only when called by trace writer
            if (HttpContext.Current == null)
            {
                return "{{SiteUrl}}";
            }

            var url = HttpContext.Current.Request.Url;
            var suffix = string.Empty;

            // if hosted in local IIS as virtual directory, add the name
            if (url.Port == 80 && !DataConfig.IsLiveMode() && url.Segments.Length >= 2)
            {
                suffix += url.Segments[1].TrimEnd('/') + "/";
            }

            return string.Format("{0}://{1}/{2}", url.Scheme, url.Authority, suffix);
        }

        public static string GetTeamFlagImage(this UrlHelper helper, string flagPrefixOrExternalID)
        {
            //if external url, add id
            if (TeamFlagUrl != null && TeamFlagUrl.StartsWith("http"))
                return TeamFlagUrl + flagPrefixOrExternalID + ".png";
            //otherwise resolve relative url and add id
            else
                return AbsoluteUrl(helper) + TeamFlagUrl + flagPrefixOrExternalID + ".svg";
        }

        public static string GetTeamFlagImage(string flagPrefixOrExternalID)
        {
            return GetTeamFlagImage(null, flagPrefixOrExternalID);
        }

        public static string GetTeamLogoImage(this UrlHelper helper, string externalID)
        {
            return TeamLogoUrl + externalID + ".png";
        }

        public static string GetTeamLogoImage(string externalID)
        {
            return GetTeamLogoImage(null, externalID);
        }

        public static string Img(this UrlHelper helper, string url, bool absolute = false)
        {
            if (absolute)
            {
                return AbsoluteUrl(helper) + ImageRoot.Replace("~/", "") + url;
            }
            else
            {
                return helper.Content(ImageRoot + url);
            }
        }

        public static string GetUserImage(this UrlHelper helper, string username)
        {
            var imgPath = "profile/" + username + ".jpg";
            var fileInfo = new FileInfo(HostingEnvironment.MapPath("~/content/img/" + imgPath));
            var root = AbsoluteUrl(helper) + ImageRoot.Replace("~/", "");

            if (fileInfo.Exists)
            {
                return root + imgPath + "?v=" + fileInfo.LastWriteTime.ToString("yyyyMMddHHmmss");
            }
            else
            {
                return root + "profile/user.svg";
            }
        }

        public static string GetUserImage(string username)
        {
            return GetUserImage(null, username);
        }
    }
}