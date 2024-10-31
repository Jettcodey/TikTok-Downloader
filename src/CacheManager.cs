using System;
using System.Collections.Generic;
using System.Diagnostics;
using Clock = System.Timers;

namespace TikTok_Downloader
{
    public class CacheManager
    {
        private List<object> cache = new List<object>();
        private Clock.Timer cleanupTimer;

        public CacheManager()
        {
            cleanupTimer = new Clock.Timer(15000);
            cleanupTimer.Elapsed += (sender, e) => CheckMemoryAndClearCache();
            cleanupTimer.Start();
        }

        public void AddToCache(object item)
        {
            cache.Add(item);
        }

        private void CheckMemoryAndClearCache()
        {
            long memoryUsage = Process.GetCurrentProcess().WorkingSet64;
            double memoryUsageMB = memoryUsage / (1024 * 1024);

            if (memoryUsageMB > 240)
            {
                ClearCache();
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }

        private void ClearCache()
        {
            cache.Clear();
            Console.WriteLine("Cache cleared to manage memory usage.");
        }
    }
}
