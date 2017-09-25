using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.WebApi;
using Hepsiburada.Zipkin.Interfaces;
using Hepsiburada.Zipkin.Models;
using Hepsiburada.Zipkin.Models.Configuration;

namespace Sample.Api1
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            var zipkinConfig = new ZipkinConfig
            {
                Domain =
                    request => new Uri(
                        "https://listing.hepsiburada.com"),                 
                ZipkinBaseUri = new Uri("http://192.168.99.100:32776"),
                SpanProcessorBatchSize = 1,
                SampleRate = 1
            };
           
            //ioc
            var builder = new ContainerBuilder();

            var config = GlobalConfiguration.Configuration;

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            builder.RegisterWebApiFilterProvider(config);

            builder.RegisterWebApiModelBinderProvider();

            builder.Register(c => new ZipkinClient(zipkinConfig, HttpContext.Current)).As<IZipkinClient>()
                .SingleInstance();

            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}
