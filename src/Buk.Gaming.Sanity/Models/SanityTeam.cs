using Buk.Gaming.Models;
using Sanity.Linq;
using Sanity.Linq.BlockContent;
using Sanity.Linq.CommonTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Buk.Gaming.Sanity.Models {
    public class SanityTeam : SanityDocument
    {
        public SanityTeam() {
            SanityType = "team";
        }

        public string Name { get; set; }

        public SanityReference<SanityOrganization> Organization { get; set; }

        public SanityReference<SanityPlayer> Captain { get; set; }

        public List<SanityReference<SanityPlayer>> Players { get; set; }

        public SanityReference<SanityGame> Game { get; set; }
    }
}