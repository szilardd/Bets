using System;
using System.Web.Http;
using Bets.Data;

namespace Bets
{
    public class CustomAuthorize : AuthorizeAttribute
    {
        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            bool needsAuthorization = false;

            // ignore authorization in debug mode if not Authorization header is provided
            if (!DataConfig.IsLiveMode())
            {
                needsAuthorization = actionContext.Request.Headers.Contains("Authorization");
            }

            if (needsAuthorization)
            {
                base.OnAuthorization(actionContext);
            }
        }
    }
}
