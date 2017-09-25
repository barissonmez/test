using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Hepsiburada.Zipkin.Interfaces;
using Hepsiburada.Zipkin.Tracer;

namespace Sample.Api2.HttpHandlers
{
    public class ZipkinTraceHandler : DelegatingHandler
    {
     
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var requestScope = request.GetDependencyScope();
            var zipkinClient = requestScope.GetService(typeof(IZipkinClient)) as IZipkinClient;
            HttpResponseMessage response;
            if (zipkinClient != null)
            {
                if (!zipkinClient.IsTraceOn)
                {
                    return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
                }

                var nextTrace = zipkinClient.TraceProvider.GetNext();

                request.Headers.Add(TraceProvider.TraceIdHeaderName, nextTrace.TraceId);
                request.Headers.Add(TraceProvider.SpanIdHeaderName, nextTrace.SpanId);
                request.Headers.Add(TraceProvider.ParentSpanIdHeaderName, nextTrace.ParentSpanId);
                request.Headers.Add(TraceProvider.SampledHeaderName, nextTrace.ParentSpanId);

                var span = zipkinClient.StartClientTrace(request.RequestUri, request.Method.ToString(), nextTrace);
                response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

                zipkinClient.EndClientTrace(span, (int) response.StatusCode);
            }
            else
            {
                response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            }
            return response;
        }
    }
}