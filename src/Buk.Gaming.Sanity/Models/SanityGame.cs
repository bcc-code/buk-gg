using Buk.Gaming.Models;
using Sanity.Linq;
using Sanity.Linq.BlockContent;
using Sanity.Linq.CommonTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Buk.Gaming.Sanity.Models {
    public class SanityGame : SanityDocument {
        public SanityGame() {
            SanityType = "game";
        }

        public string Name { get; set; }

        public bool HasTeams { get; set; }

        public SanityImage Icon { get; set; }
    }
}