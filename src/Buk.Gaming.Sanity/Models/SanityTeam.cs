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

        public SanityTeam(Team team)
        {
            Name = team.Name;
            Id = team.Id;
            Organization = new SanityReference<SanityOrganization>()
            {
                Ref = team.OrganizationId,
            };
            Captain = new SanityReference<Player>()
            {
                Ref = team.CaptainId,
            };
            Game = new SanityReference<SanityGame>()
            {
                Ref = team.GameId,
            };
            Players = team.PlayerIds.Select(i => new SanityReference<Player>()
            {
                Ref = i,
            }).ToList();
        }

        public string Name { get; set; }

        public SanityReference<SanityOrganization> Organization { get; set; }

        public SanityReference<Player> Captain { get; set; }

        public List<SanityReference<Player>> Players { get; set; }

        public SanityReference<SanityGame> Game { get; set; }

        public Team ToTeam() => new Team()
        {
            CaptainId = Captain.Ref,
            GameId = Game.Ref,
            OrganizationId = Organization.Ref,
            Id = Id,
            Name = Name,
            PlayerIds = Players.Select(i => i.Ref).ToList()
        };
    }
}