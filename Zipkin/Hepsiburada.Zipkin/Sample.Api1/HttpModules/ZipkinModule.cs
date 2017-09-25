using System;
using System.Web;
using Hepsiburada.Zipkin.Models;
using Hepsiburada.Zipkin.Models.Spans;

namespace Sample.Api1.HttpModules
{
    public class ZipkinModule : IHttpModule
    {
        private readonly ZipkinClient _client;
        public ZipkinModule(ZipkinClient client)
        {
            _client = client;
        }

        public String ModuleName
        {
            get { return "ZipkinModule"; }
        }

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
            HttpApplication application = (HttpApplication)source;
            HttpContext context = application.Context;
            Span span = _client.StartServerTrace(context.Request.Url, context.Request.HttpMethod);
            context.Items.Add("span", span);
        }

        private void Application_EndRequest(Object source, EventArgs e)
        {
            HttpApplication application = (HttpApplication)source;
            HttpContext context = application.Context;
            var spanData = context.Items["span"];
            if (spanData != null)
            {
                var span = spanData as Span;
                _client.EndServerTrace(span);
                context.Items.Remove("span");
            }
       
        }

        public void Dispose() { }
    }
}
