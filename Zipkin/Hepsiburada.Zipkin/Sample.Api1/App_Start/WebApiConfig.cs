using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Sample.Api.Core.Handlers;
using Sample.Api1.HttpHandlers;

namespace Sample.Api1
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.MessageHandlers.Add(new ApiResponseHandler());
            config.MessageHandlers.Add(new ZipkinTraceHandler());
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
