using Buk.Gaming.Models;
using Sanity.Linq;
using Sanity.Linq.BlockContent;
using Sanity.Linq.CommonTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Buk.Gaming.Sanity.Models
{
    public class SanityCamp : SanityDocument
    {
        public SanityCamp()
        {
            SanityType = "camp";
        }

        public SanityLocaleString Title { get; set; }

        public SanityLocaleString Decsription { get; set; }

        public bool Active { get; set; }

    }
}
