using Sanity.Linq.CommonTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Buk.Gaming.Sanity.Models
{
    public class SanityPage : SanityDocument
    {
        public SanityPage()
        {
            SanityType = "page";
        }

        public SanitySlug Slug { get; set; }

        public SanityImage MainImage { get; set; }

        public SanityImage Logo { get; set; }

        public SanityLocaleString Title { get; set; }

        public SanityLocaleBlock Introduction { get; set; }

        public SanityLocaleBlock Body { get; set; }

    }
}
