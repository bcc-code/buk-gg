using Buk.Gaming.Models;
using Sanity.Linq;
using Sanity.Linq.BlockContent;
using Sanity.Linq.CommonTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Buk.Gaming.Sanity.Models
{
    public class SanityEvent : SanityDocument
    {
        public SanityEvent()
        {
            SanityType = "event";
        }

        public SanityLocaleString Title { get; set;}

        public SanitySlug Slug { get; set; }

        [Include]
        public SanityImage Image { get; set; }

        public SanityReference<SanityCategory> Category { get; set; }

        [Include]
        public SanityReference<Player> Responsible { get; set; }

        public DateTime Date { get; set; }

        public SanityLocaleBlock Description { get; set; }

        public bool LiveChat { get; set; }
    }
}
