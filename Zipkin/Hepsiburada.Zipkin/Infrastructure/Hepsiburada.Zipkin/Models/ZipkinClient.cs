using System;
using System.Runtime.CompilerServices;
using System.Web;
using Hepsiburada.Zipkin.Collectors;
using Hepsiburada.Zipkin.Constants;
using Hepsiburada.Zipkin.Interfaces;
using Hepsiburada.Zipkin.Models.Endpoints;
using Hepsiburada.Zipkin.Models.Spans;
using Hepsiburada.Zipkin.Tracer;
using Hepsiburada.Zipkin.Utils;

namespace Hepsiburada.Zipkin.Models
{
    public class ZipkinClient: IZipkinClient
    {
        internal SpanCollector spanCollector;
        internal SpanTracer spanTracer;

        public bool IsTraceOn { get; set; }

        public ITraceProvider TraceProvider { get; }

        public IZipkinConfig ZipkinConfig { get; }

        private static SpanCollector instance;
        private static readonly object syncObj = new object();

        static SpanCollector GetInstance(Uri uri, uint maxProcessorBatchSize)
        {
            TaskHelper.Execute(syncObj, () => instance == null,
                () =>
                    {
                        instance = new SpanCollector(uri, maxProcessorBatchSize);
                    });

            return instance;
        }

        public ZipkinClient(IZipkinConfig zipkinConfig, HttpContext context, SpanCollector collector = null)
        {
            if (zipkinConfig == null) throw new ArgumentNullException(nameof(zipkinConfig));
            if (context == null) throw new ArgumentNullException(nameof(context));
            var traceProvider = new TraceProvider(zipkinConfig, context);
            IsTraceOn = !zipkinConfig.Bypass(context.Request) && IsTraceProviderSamplingOn(traceProvider);

            if (!IsTraceOn)
                return;

            zipkinConfig.Validate();
            ZipkinConfig = zipkinConfig;
            try
            {
                spanCollector = collector ?? GetInstance(
                    zipkinConfig.ZipkinBaseUri,
                    zipkinConfig.SpanProcessorBatchSize);

                spanCollector.Start();

                spanTracer = new SpanTracer(
                    spanCollector,
                    new ServiceEndpoint(),
                    zipkinConfig.NotToBeDisplayedDomainList,
                    zipkinConfig.Domain(context.Request));

                TraceProvider = traceProvider;
            }
            catch (Exception ex)
            {
                IsTraceOn = false;
            }
        }

        public Span StartClientTrace(Uri remoteUri, string methodName, ITraceProvider trace)
        {
            if (!IsTraceOn)
                return null;

            try
            {
                return spanTracer.SendClientSpan(
                    methodName.ToLower(),
                    trace.TraceId,
                    trace.ParentSpanId,
                    trace.SpanId,
                    remoteUri);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public void EndClientTrace(Span clientSpan, int statusCode)
        {
            if (!IsTraceOn)
                return;

            try
            {
                spanTracer.ReceiveClientSpan(clientSpan, statusCode);
            }
            catch (Exception ex)
            {
            }
        }

        public Span StartServerTrace(Uri requestUri, string methodName)
        {
            if (!IsTraceOn)
                return null;

            try
            {
                return spanTracer.ReceiveServerSpan(
                    methodName.ToLower(),
                    TraceProvider.TraceId,
                    TraceProvider.ParentSpanId,
                    TraceProvider.SpanId,
                    requestUri);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public void EndServerTrace(Span serverSpan)
        {
            if (!IsTraceOn)
                return;

            try
            {
                spanTracer.SendServerSpan(serverSpan);
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// Records an annotation with the current timestamp and the provided value in the span.
        /// </summary>
        /// <param name="span">The span where the annotation will be recorded.</param>
        /// <param name="value">The value of the annotation to be recorded. If this parameter is omitted
        /// (or its value set to null), the method caller member name will be automatically passed.</param>
        public void Record(Span span, [CallerMemberName] string value = null)
        {
            if (!IsTraceOn)
                return;

            try
            {
                spanTracer.Record(span, value);
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// Records a key-value pair as a binary annotiation in the span.
        /// </summary>
        /// <typeparam name="T">The type of the value to be recorded. See remarks for the currently supported types.</typeparam>
        /// <param name="span">The span where the annotation will be recorded.</param>
        /// <param name="key">The key which is a reference to the recorded value.</param>
        /// <param name="value">The value of the annotation to be recorded.</param>
        /// <remarks>The RecordBinary will record a key-value pair which can be used to tag some additional information
        /// in the trace without any timestamps. The currently supported value types are <see cref="bool"/>,
        /// <see cref="byte[]"/>, <see cref="short"/>, <see cref="int"/>, <see cref="long"/>, <see cref="double"/> and
        /// <see cref="string"/>. Any other types will be passed as string annotation types.
        /// 
        /// Please note, that although the values have types, they will be recorded and sent by calling their
        /// respective ToString() method.</remarks>
        public void RecordBinary<T>(Span span, string key, T value)
        {
            if (!IsTraceOn)
                return;

            try
            {
                spanTracer.RecordBinary<T>(span, key, value);
            }
            catch (Exception ex)
            {
               
            }
        }

        /// <summary>
        /// Records a local component annotation in the span.
        /// </summary>
        /// <param name="span">The span where the annotation will be recorded.</param>
        /// <param name="value">The value of the local trace to be recorder.</param>
        public void RecordLocalComponent(Span span, string value)
        {
            if (!IsTraceOn)
                return;

            try
            {
                spanTracer.RecordBinary(span, ZipkinConstants.LocalComponent, value);
            }
            catch (Exception ex)
            {
           
            }
        }

        public void ShutDown()
        {
            if (spanCollector != null)
            {
                spanCollector.Stop();
            }
        }

        private bool IsTraceProviderSamplingOn(ITraceProvider traceProvider)
        {
            return !string.IsNullOrEmpty(traceProvider.TraceId) && traceProvider.IsSampled;
        }
    }
}
