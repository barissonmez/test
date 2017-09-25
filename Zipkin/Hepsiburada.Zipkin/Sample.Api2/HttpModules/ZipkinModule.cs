using System;
using System.Web;
using Hepsiburada.Zipkin.Interfaces;
using Hepsiburada.Zipkin.Models;
using Hepsiburada.Zipkin.Models.Configuration;
using Hepsiburada.Zipkin.Models.Spans;

namespace Sample.Api2.HttpModules
{
    public class ZipkinModule : IHttpModule
    {
    
        public String ModuleName
        {
            get { return "ZipkinModule"; }
        }

        private IZipkinClient _client;

        public void Init(HttpApplication application)
        {
            application.BeginRequest +=
                (this.Application_BeginRequest);
            application.EndRequest +=
                (this.Application_EndRequest);

           
        }

        private void Application_BeginRequest(Object source,
            EventArgs e)
        {
            HttpApplication application = (HttpApplication) source;
            HttpContext context = application.Context;
            if (_client == null && HttpContext.Current != null && HttpContext.Current.Handler != null)
            {
                var zipkinConfig = new ZipkinConfig
                {
                    Domain =
                        request => new Uri(
                            "https://product.hepsiburada.com"),
                    ZipkinBaseUri = new Uri("http://192.168.99.100:32772"),
                    SpanProcessorBatchSize = 1,
                    SampleRate = 1
                };

                _client = new ZipkinClient(zipkinConfig, HttpContext.Current);
            }
            if (_client != null)
            {
                Span span = _client.StartServerTrace(context.Request.Url, context.Request.HttpMethod);
                context.Items.Add("span", span);
            }
        }

        private void Application_EndRequest(Object source, EventArgs e)
        {
            HttpApplication application = (HttpApplication) source;
            HttpContext context = application.Context;
            if (_client == null && HttpContext.Current != null && HttpContext.Current.Handler != null)
            {
                var zipkinConfig = new ZipkinConfig
                {
                    Domain =
                        request => new Uri(
                            "https://product.hepsiburada.com"),
                    ZipkinBaseUri = new Uri("http://192.168.99.100:32772"),
                    SpanProcessorBatchSize = 1,
                    SampleRate = 1
                };

                _client = new ZipkinClient(zipkinConfig, HttpContext.Current);
            }
            if (_client != null)
            {
                var spanData = context.Items["span"];
                if (spanData != null)
                {
                    var span = spanData as Span;
                    _client.EndServerTrace(span);
                    context.Items.Remove("span");
                }
            }
        }

        public void Dispose() { }
    }
}
