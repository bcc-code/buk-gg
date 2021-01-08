using Sanity.Linq.CommonTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Buk.Gaming.Sanity.Models
{
    public class SanityImageWithCaption : SanityImage
    {
        public SanityLocaleString Caption { get; set; }
    }
}
