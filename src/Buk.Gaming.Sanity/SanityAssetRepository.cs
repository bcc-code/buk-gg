using Buk.Gaming.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Sanity.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buk.Gaming.Sanity
{
    public class AssetRepository : SanityRepository
    {
        public AssetRepository(SanityDataContext sanity, IMemoryCache cache): base(sanity, cache)
        {

        }

        public async Task<string> UploadImageAsync(Stream image, string filename)
        {
            var doc = await Sanity.Images.UploadAsync(image, filename);

            return doc.Document.Id;
        }
    }
}
