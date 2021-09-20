using Buk.Gaming.Models;
using Buk.Gaming.Repositories;
using Buk.Gaming.Sanity.Models;
using Microsoft.Extensions.Caching.Memory;
using Sanity.Linq;
using Sanity.Linq.Extensions;
using Sanity.Linq.CommonTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Concurrent;
using System.Threading;

namespace Buk.Gaming.Sanity
{
    public abstract class SanityRepository
    {
        protected ConcurrentDictionary<string, SemaphoreSlim> Semaphores = new();

        protected SanityRepository(SanityDataContext sanity, IMemoryCache cache)
        {
            Sanity = sanity;
            Cache = cache;
        }

        protected readonly SanityDataContext Sanity;

        protected readonly IMemoryCache Cache;

        protected async Task UpdateDocumentAsync<T>(string documentId, Action<T> factory) where T : SanityDocument
        {
            SemaphoreSlim semaphore = Semaphores.GetOrAdd($"UPDATE_DOCUMENT_{documentId}", new SemaphoreSlim(1, 1));

            try
            {
                await semaphore.WaitAsync();

                T item = await Sanity.DocumentSet<T>().GetAsync(documentId);

                if (item == null)
                {
                    throw new Exception("Item not found");
                }

                factory(item);

                await Sanity.DocumentSet<T>().Update(item).CommitAsync();
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}
