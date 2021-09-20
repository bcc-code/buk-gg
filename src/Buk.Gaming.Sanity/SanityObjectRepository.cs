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
using Buk.Gaming.Sanity.Extensions;

namespace Buk.Gaming.Sanity {
    public class SanityObjectRepository : SanityRepository, IObjectRepository
    {
        public SanityObjectRepository(SanityDataContext sanity, IMemoryCache cache): base(sanity, cache)
        {

        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            return (await Sanity.DocumentSet<SanityCategory>().ToListAsync()).Select(i => i.ToCategory()).ToList();
        }

        public async Task<List<Game>> GetGamesAsync()
        {
            return (await Sanity.DocumentSet<SanityGame>().ToListAsync()).Select(i => i.ToGame()).ToList();
        }

        public async Task<List<Camp>> GetCampsAsync()
        {
            return (await Sanity.DocumentSet<SanityCamp>().ToListAsync()).Select(i => i.ToCamp()).ToList();
        }
    }
}