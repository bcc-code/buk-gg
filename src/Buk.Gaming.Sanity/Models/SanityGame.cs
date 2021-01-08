using Buk.Gaming.Models;
using Sanity.Linq;
using Sanity.Linq.BlockContent;
using Sanity.Linq.CommonTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Buk.Gaming.Sanity.Models {
    public class SanityGame : SanityDocument {
        SanityGame() {
            SanityType = "game";
        }

        public string Name { get; set; }

        public bool HasTeams { get; set; }

        [Include]
        public SanityImage Icon { get; set; }

        public SanityObject[] TeamFields { get; set; }

        public SanityObject[] PlayerFields { get; set; }

        public Game ToGame() {
            return new Game() {
                Id = this.Id,
                HasTeams = this.HasTeams,
                Icon = this.Icon?.Asset?.Value?.Url,
                Name = this.Name
            };
        }
    }
}