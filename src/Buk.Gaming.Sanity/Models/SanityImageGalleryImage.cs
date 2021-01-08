using Sanity.Linq.CommonTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Buk.Gaming.Sanity.Models
{
    public class SanityImageGalleryImage : SanityImage
    {
        public SanityBlock[] Caption { get; set; }

    }
}
