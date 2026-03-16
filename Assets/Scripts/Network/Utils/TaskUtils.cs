using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Network.Utils
{
    public static class TaskUtils
    {
        public static async Task Delay(float seconds, CancellationToken ct = default)
        {
            var t = seconds;
            
            while (t > 0)
            {
                await Task.Yield();

                if (ct.IsCancellationRequested)
                {
                    break;
                }
                
                t -= Time.deltaTime;
            }
        }

        public static async Task WaitUntil( 
            Func<bool> isCompleted, 
            CancellationToken ct = default)
        {
            while (!isCompleted.Invoke())
            {
                await Task.Yield();

                if (ct.IsCancellationRequested)
                {
                    break;
                }
            }
        }
        
        public static void FireAndForget(
            this Task task,
            Action<Exception>? onException = null)
        {
            task.ContinueWith(
                t =>
                {
                    if (t.Exception != null)
                    {
                        onException?.Invoke(t.Exception.GetBaseException());
                    }
                },
                TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}