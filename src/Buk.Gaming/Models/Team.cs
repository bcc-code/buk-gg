using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Buk.Gaming.Models
{
    public class Team
    {
        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("_type")]
        public string Type => "team";

        public string Name { get; set; }

        public string OrganizationId { get; set; }

        public string GameId { get; set; }

        public string CaptainId { get; set; }

        public List<string> PlayerIds { get; set; }

        public Public View(List<Player> players) => new Public(this, players);

        public class Public
        {
            public Public(Team team, List<Player> players)
            {
                Id = team.Id;
                Name = team.Name;
                OrganizationId = team.OrganizationId;
                GameId = team.GameId;
                Captain = players.Find(p => team.CaptainId == p.Id).View();
                Players = players.Where(p => p.Id != Captain.Id).Select(i => i.View()).ToList();
            }

            public string Id { get; set; }

            public string Name { get; set; }

            public string OrganizationId { get; set; }

            public string GameId { get; set; }

            public Player.Public Captain { get; set; }

            public List<Player.Public> Players { get; set; }
        }
    }
}