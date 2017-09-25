using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Hepsiburada.Zipkin.Utils;

namespace Hepsiburada.Zipkin.Processors
{
    public class SpanProcessorTaskFactory
    {
        private Task spanProcessorTaskInstance;
        private CancellationTokenSource cancellationTokenSource;
        private const int defaultDelayTime = 500;
        private const int encounteredAnErrorDelayTime = 30000;

        readonly object sync = new object();

        public SpanProcessorTaskFactory(CancellationTokenSource cancellationTokenSource)
        {
            this.cancellationTokenSource = cancellationTokenSource ?? new CancellationTokenSource();
        }

        public SpanProcessorTaskFactory()
            :this(new CancellationTokenSource())
        {
        }

        [ExcludeFromCodeCoverage]  //excluded from code coverage since this class is a 1 liner that starts up a background thread
        public virtual void CreateAndStart(Action action)
        {
            TaskHelper.Execute(sync, () => spanProcessorTaskInstance == null || spanProcessorTaskInstance.Status == TaskStatus.Faulted,
                () =>
                {
                    spanProcessorTaskInstance = Task.Factory.StartNew(() => ActionWrapper(action), cancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
                });
        }

        public virtual void StopTask()
        {
            TaskHelper.Execute(sync, () => cancellationTokenSource.Token.CanBeCanceled, () => cancellationTokenSource.Cancel());
        }

        internal async void ActionWrapper(Action action)
        {
            while (!IsTaskCancelled())
            {
                int delayTime = defaultDelayTime;
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    delayTime = encounteredAnErrorDelayTime;
                }

                // stop loop if task is cancelled while delay is in process
                try
                {
                    await Task.Delay(delayTime, cancellationTokenSource.Token);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
                
            }
        }

        public virtual bool IsTaskCancelled()
        {
            return cancellationTokenSource.IsCancellationRequested;
        }
    }
}
