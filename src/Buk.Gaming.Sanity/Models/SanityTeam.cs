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

        [Include]
        public SanityReference<SanityOrganization> Organization { get; set; }

        [Include]
        public SanityReference<Player> Captain { get; set; }

        [Include]
        public List<SanityReference<Player>> Players { get; set; }

        [Include]
        public SanityReference<SanityGame> Game { get; set; }

        public Team ToTeam()
        {
            Game game = new Game
            {
                Name = Game.Value.Name,
                Id = Game.Ref,
                HasTeams = Game.Value.HasTeams,
                Icon = Game.Value.Icon?.Asset.Value?.Url
            };
            return new Team()
            {
                Id = Id,
                Name = Name,
                Organization = Organization.Value.ToOrganization(),
                Game = game,
                Captain = Captain.Value,
                Players = Players.Select(p => p.Value).ToList(),
            };
        }
    }
}