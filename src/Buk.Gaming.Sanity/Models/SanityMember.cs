using Sanity.Linq.CommonTypes;
using Buk.Gaming.Models;
using Sanity.Linq;
using Sanity.Linq.BlockContent;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;


namespace Buk.Gaming.Sanity.Models
{
    public class SanityMember : SanityObject
    {
        public string Role { get; set; }

        [Include]
        public SanityReference<SanityPlayer> Player { get; set; }
    }
}
