using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Security;
using Bets.Data;
using AutoMapper;
using System.Reflection;
using Autofac;
using ThatAuthentication;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using Bets.Infrastructure;
using System.Diagnostics;

namespace Bets
{
    public static class Bootstrapper
    {
        public static void RegisterDependencyInjection()
        {
            var builder = new ContainerBuilder();
            var config = GlobalConfiguration.Configuration;

            // register controllers
            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            builder.RegisterApiControllers(typeof(MvcApplication).Assembly);

            // register model binders that require DI.
            builder.RegisterModelBinders(Assembly.GetExecutingAssembly());
            builder.RegisterModelBinderProvider();

            // register web abstractions like HttpContextBase.
            builder.RegisterModule<AutofacWebTypesModule>();

            builder.RegisterType<UserRepository>().As<IUserRepository>();

            builder.Register<IMapper>(b =>
            {
                return AutomapperConfig.Register();
            }).SingleInstance();

            // set the MVC dependency resolver to be Autofac.
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            // set the WebAPI dependency resolver to be Autofac.
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}