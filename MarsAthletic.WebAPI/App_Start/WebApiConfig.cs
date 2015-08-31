using System;
using System.Collections.Generic;
using System.Net.Http.Formatting;
using System.Linq;
using System.Web.Http;
using Microsoft.Practices.Unity;
using MarsAthletic.WebAPI.Interfaces;
using MarsAthletic.WebAPI.Helpers;
using MarsAthletic.WebAPI.Resolver;
using MarsAthletic.WebAPI.Providers;

namespace MarsAthletic.WebAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.EnableCors();


            var container = new UnityContainer();

            container.RegisterType<IOperations, OperationsHelper>(new HierarchicalLifetimeManager());
            container.RegisterType<IConfigHelper, ConfigHelper>(new HierarchicalLifetimeManager());

            config.DependencyResolver = new UnityResolver(container);

            config.Formatters.XmlFormatter.AddUriPathExtensionMapping(".xml", "application/xml");
            config.Formatters.JsonFormatter.AddUriPathExtensionMapping(".json", "application/json");

            // Web API routes
            config.MapHttpAttributeRoutes();


            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "Rest/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );



        }
    }
}
