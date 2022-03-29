using Buk.Gaming.Models;
using Sanity.Linq;
using Sanity.Linq.BlockContent;
using Sanity.Linq.CommonTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Buk.Gaming.Sanity.Models
{
    public class SanityCategory : SanityDocument
    {
        public SanityCategory()
        {
            SanityType = "category";
        }

        public SanityLocaleString Title { get; set; }

        public SanityLocaleString Decsription { get; set; }

    }
}
