using Buk.Gaming.Models;
using Buk.Gaming.Repositories;
using Buk.Gaming.Sanity.Models;
using Microsoft.Extensions.Caching.Memory;
using Sanity.Linq;
using Sanity.Linq.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buk.Gaming.Sanity
{
    public class SanityEventRepository : IEventRepository
    {
        public SanityEventRepository(SanityDataContext sanity, IMemoryCache cache)
        {
            Sanity = sanity;
            Cache = cache;
        }

        public SanityDataContext Sanity { get; }
        public IMemoryCache Cache { get; }

        public Task<List<EventInfo>> GetAllEventsAsync()
        {
            var lang = System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
            return Cache.GetOrCreateAsync("Sanity_All_Events_" + lang, async (c) => {
                c.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10);
                var events = await Sanity.DocumentSet<SanityEvent>().Where(e => !e.IsDraft()).ToListAsync();
                return events?.Select(t => t.ToEventInfo(Sanity.HtmlBuilder)).ToList();
            });
        }

        public Task<EventInfo> GetEventInfoAsync(string eventId)
        {
            var lang = System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
            return Cache.GetOrCreateAsync("Sanity_Event_Info_" + lang, async (c) =>
            {
                c.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10);
                var result = await Sanity.DocumentSet<SanityEvent>().Where(e => e.Id == eventId || e.Slug.Current == eventId).FirstOrDefaultAsync();
                EventInfo eventI = result.ToEventInfo(Sanity.HtmlBuilder);
                return eventI;
            });
        }
    }
}
