using System;

namespace Hepsiburada.Zipkin.Utils
{
    public static class TaskHelper
    {
        public static void Execute(object sync, Func<bool> canExecute, Action actiontoExecuteSafely)
        {
            if (canExecute())
            {
                lock (sync)
                {
                    if (canExecute())
                    {
                        actiontoExecuteSafely();
                    }
                }
            }
        }
    }
}
