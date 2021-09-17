using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Buk.Gaming.Web.Classes
{
    public static class SemaphoreHelper
    {
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> _semaphores = new ConcurrentDictionary<string, SemaphoreSlim>();

        public static async Task<T> WithSemaphoreAsync<T>(this IMemoryCache memoryCache, string key, Func<Task<T>> factory, TimeSpan? expiry = null)
        {
            if (!memoryCache.TryGetValue(key, out T result))
            {
                var semaphore = _semaphores.GetOrAdd(key, new SemaphoreSlim(1, 1));

                try
                {
                    await semaphore.WaitAsync();

                    if (!memoryCache.TryGetValue(key, out result))
                    {
                        result = await factory();

                        if (expiry != null)
                        {
                            memoryCache.Set(key, result, (TimeSpan)expiry);
                        } else
                        {
                            memoryCache.Set(key, result);
                        }
                    }
                }
                finally
                {
                    semaphore.Release();
                }
            }
            return result;
        }
    }
}
