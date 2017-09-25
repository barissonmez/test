using System.Diagnostics;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Hepsiburada.Zipkin.Interfaces;
using Hepsiburada.Zipkin.Tracer;

namespace Sample.Api1.ActionFilters
{
    public class ZipkinTraceAttribute : ActionFilterAttribute
    {
   

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            base.OnActionExecuting(actionContext);
            var requestScope = actionContext.Request.GetDependencyScope();
            var zipkinClient = requestScope.GetService(typeof(IZipkinClient)) as IZipkinClient;
           
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            base.OnActionExecuted(actionExecutedContext);
            var requestScope = actionExecutedContext.Request.GetDependencyScope();
            var zipkinClient = requestScope.GetService(typeof(IZipkinClient)) as IZipkinClient;
        }
    }
}
