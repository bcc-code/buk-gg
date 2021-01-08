using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
namespace Buk.Gaming.Models
{
    public class Team
    {
        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("_type")]
        public string Type => "team";

        public string Name { get; set; }

        public Organization Organization { get; set; }

        public Player Captain { get; set; }

        public Player[] Players { get; set; }

        public Game Game { get; set; }
    }
}