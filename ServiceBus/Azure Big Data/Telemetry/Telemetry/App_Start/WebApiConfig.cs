using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Http;
using Telemetry.Api.DelegateHandler;
using Telemetry.Api.ExceptionFilters;

namespace Telemetry
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "device-events",
                routeTemplate: "events",
                defaults: new {Controller = "Events"},
                handler: new GZipToJsonHandler(GlobalConfiguration.Configuration),
                constraints: null
                );

            config.Filters.Add(new LoggingExceptionFilterAttribute());
            config.MessageHandlers.Add(new StandardResponseHeaderHandlers());
        }
    }
}
