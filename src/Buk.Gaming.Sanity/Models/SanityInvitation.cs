using Sanity.Linq.CommonTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Buk.Gaming.Sanity.Models
{
    public class SanityInvitation : SanityObject
    {
        public SanityReference<SanityPlayer> Player { get; set; }

        public string Type { get; set; }
    }
}
