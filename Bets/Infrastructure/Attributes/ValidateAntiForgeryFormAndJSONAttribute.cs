using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections.Specialized;
using System.Web.Helpers;

namespace Bets
{
    /// <summary>
    /// Implements anti forgery token validation for AJAX requests. Works with normal POST (form element) also
    /// http://haacked.com/archive/2011/10/10/preventing-csrf-with-ajax.aspx
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ValidateAntiForgeryFormAndJSONAttribute : FilterAttribute, IAuthorizationFilter
    {
        public static string TokenName = "__RequestVerificationToken";

        private class JsonAntiForgeryHttpContextWrapper : HttpContextWrapper
        {
            readonly HttpRequestBase _request;

            public JsonAntiForgeryHttpContextWrapper(HttpContext httpContext)
                : base(httpContext)
            {
                _request = new JsonAntiForgeryHttpRequestWrapper(httpContext.Request);
            }

            public override HttpRequestBase Request
            {
                get
                {
                    return _request;
                }
            }
        }

        private class JsonAntiForgeryHttpRequestWrapper : HttpRequestWrapper
        {
            readonly NameValueCollection _form;

            public JsonAntiForgeryHttpRequestWrapper(HttpRequest request)
                : base(request)
            {
                _form = new NameValueCollection(request.Form);

                if (request.Headers[TokenName] != null)
                    _form[TokenName] = request.Headers[TokenName];
            }

            public override NameValueCollection Form
            {
                get
                {
                    return _form;
                }
            }
        }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
                throw new ArgumentNullException("filterContext");

            var httpContext = filterContext.HttpContext;
            var cookie = httpContext.Request.Cookies[AntiForgeryConfig.CookieName];
            var token = httpContext.Request.Form[TokenName] ?? httpContext.Request.Headers[TokenName];

            AntiForgery.Validate(cookie == null ? null : cookie.Value, token);
        }

        public string Salt
        {
            get;
            set;
        }

        // The private context classes go here
    }
}