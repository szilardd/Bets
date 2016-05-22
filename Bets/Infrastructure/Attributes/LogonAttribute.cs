using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bets.Controllers;

namespace Bets
{
    public class LogonAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!(filterContext.Controller is AccountController || filterContext.Controller is NotificationController || filterContext.Controller is ErrorController))
                base.OnAuthorization(filterContext);
        }
    }
}