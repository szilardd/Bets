using System;
using System.Web.Http;
using System.Web.Http.OData.Extensions;
using System.Web.Http.ExceptionHandling;
using System.Web.OData.Builder;
using Elmah.Contrib.WebApi;
using Microsoft.Owin.Security.OAuth;
using Bets.Data;
using Bets.Infrastructure;

namespace Bets
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // configure Web API to use only bearer token authentication.
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            // routes
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // authorize using custom logic
            config.Filters.Add(new CustomAuthorize());

            if (DataConfig.EnableTracing())
            {
                //log requests
                config.Filters.Add(new ApiLogFilter());

                //enable tracing (configured in web.config)
                config.EnableSystemDiagnosticsTracing();

                //log all method calls
                //config.MessageHandlers.Add(new ApiLogHandler());
            }

            // set up elmah
            config.Services.Add(typeof(IExceptionLogger), new ElmahExceptionLogger());

            // set up global exception handler
            config.Services.Replace(typeof(IExceptionHandler), new ApiGlobalExceptionHandler());

            // set up json serialization to serialize classes to camel case
            var formatter = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
            formatter.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();

            // remove xml formatter
            var formatters = GlobalConfiguration.Configuration.Formatters;
            formatters.Remove(formatters.XmlFormatter);
        }
    }
}