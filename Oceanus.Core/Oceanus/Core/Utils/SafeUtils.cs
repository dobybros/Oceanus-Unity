using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Oceanus.Core.Utils
{
    public class SafeUtils
    {
        private static readonly string TAG = typeof(SafeUtils).Name;
        public static void SafeCallback(string actionName, Action action)
        {
            OceanusLogger.info(TAG, "SafeCallback " + actionName);
            try
            {
                action.Invoke();
            }
            catch (Exception e)
            {
                
                OceanusLogger.error(TAG, "SafeCallback {0} invoke failed, {1}, stackTrace {2}", actionName, e.Message, e.StackTrace);
            }

        }

        public static long CurrentTimeMillis()
        {
            return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }

        public static async Task WaitTimeout(int seconds, Action action)
        {
            await WaitTimeout(seconds, action, null);
        }

        public static async Task<bool> WaitTimeout(int seconds, Action action, CancellationTokenSource cts)
        {
            try
            {
                if (cts != null)
                {
                    await Task.Delay(TimeSpan.FromSeconds(seconds), cts.Token);
                }
                else
                {
                    await Task.Delay(TimeSpan.FromSeconds(seconds));
                }
            }
            catch (OperationCanceledException e)
            {
                OceanusLogger.info(TAG, "WaitTimeout secondsd " + seconds + " is canceled " + e.Message);
                return false;
            }
            SafeUtils.SafeCallback("WaitTimeout seconds " + seconds + " executed", () =>
            {
                if(!cts.Token.IsCancellationRequested)
                    action.Invoke();
            });
            return true;
        }
        public static string StreamToString(Stream stream, Encoding encode)
        {
            string ret;
            using (var reader = new StreamReader(stream, encode))
            {
                ret = reader.ReadToEnd();
            }
            stream.Close();
            return ret;
        }
    }
}
