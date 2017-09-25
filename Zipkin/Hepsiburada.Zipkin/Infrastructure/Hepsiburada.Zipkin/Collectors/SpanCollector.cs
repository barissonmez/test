using System;
using System.Collections.Concurrent;
using Hepsiburada.Zipkin.Models.Spans;
using Hepsiburada.Zipkin.Processors;
using Hepsiburada.Zipkin.Utils;

namespace Hepsiburada.Zipkin.Collectors
{
    public class SpanCollector
    {
        private const int MAX_QUEUE_SIZE = 100;
        internal static BlockingCollection<Span> spanQueue;

        internal SpanProcessor spanProcessor;

        public bool IsStarted { get; private set; }
        private readonly object syncObj = new object();

        public SpanCollector(Uri uri, uint maxProcessorBatchSize)
        {
            if (spanQueue == null)
            {
                spanQueue = new BlockingCollection<Span>(MAX_QUEUE_SIZE);
            }

            spanProcessor = new SpanProcessor(uri, spanQueue, maxProcessorBatchSize);
        }

        public virtual void Collect(Span span)
        {
            spanQueue.Add(span);
        }

        public virtual void Start()
        {
            TaskHelper.Execute(syncObj, () => !IsStarted, () => { spanProcessor.Start(); IsStarted = true; });
        }

        public virtual void Stop()
        {
            TaskHelper.Execute(syncObj, () => IsStarted, () => { spanProcessor.Stop() ; IsStarted = false; });
        }
    }
}
