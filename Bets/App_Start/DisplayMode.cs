using System;
using System.Linq;
using System.Web.WebPages;
using System.Text.RegularExpressions;
using Autofac;

namespace Bets
{
    public static class DisplayMode
    {
        /// <summary>
        /// Registers display mode for iPhone, iPod and Android phones, but no tablets
        /// </summary>
        public static void Register()
        {
            //remove Mobile provider
            DisplayModeProvider.Instance.Modes.Remove(DisplayModeProvider.Instance.Modes.Where(e => e.DisplayModeId == "Mobile").First());

            //add new Mobile provider
            DisplayModeProvider.Instance.Modes.Insert(0, new DefaultDisplayMode("Mobile")
            {
                ContextCondition =
                (
                    context =>
                    {
                        var userAgent = context.Request.UserAgent;

                        if (String.IsNullOrEmpty(userAgent))
                            return false;

                        userAgent = userAgent.ToLower();

                        return userAgent.Contains("windows phone")                                 //windows phone
                                ||
                                Regex.IsMatch(userAgent, "iphone|ipod")                             //iPhone and iPod
                                ||
                                (userAgent.Contains("android") && userAgent.Contains("mobile"));    //Android but not tablets
                    }
                )
            });
        }
    }
}