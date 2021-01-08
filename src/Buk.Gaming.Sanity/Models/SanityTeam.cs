using Buk.Gaming.Models;
using Sanity.Linq;
using Sanity.Linq.BlockContent;
using Sanity.Linq.CommonTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Buk.Gaming.Sanity.Models {
    public class SanityTeam : SanityDocument
    {
        public SanityTeam() {
            SanityType = "team";
        }

        public string Name { get; set; }

        [Include]
        public SanityReference<SanityOrganization> Organization { get; set; }

        [Include]
        public SanityReference<Player> Captain { get; set; }

        [Include]
        public List<SanityReference<Player>> Players { get; set; }

        public List<SanityMember> Members { get; set; }

        [Include]
        public SanityReference<SanityGame> Game { get; set; }
    }
}