using Newtonsoft.Json;
using System.Collections.Generic;

namespace Buk.Gaming.Models
{
    public class Organization
    {
		[JsonProperty("_id")]
        public string Id { get; set; }
        [JsonProperty("_type")]
		public string Type => "organization";

        public bool IsPublic { get; set; }

        public string Name { get; set; }

        public string Image { get; set; }

        public List<Member> Members { get; set; }

        public List<PendingMember> Pending { get; set; }
    }

    public class Member {
        [JsonProperty("_key")]
        public string Key { get; set; }

        public Player Player { get; set; }

        public string Role { get; set; }

        public Member(Player player, string role, string key = null)
        {
            this.Player = player;
            this.Role = role;
            this.Key = key;
        }
    }

    public class PendingMember
    {
        [JsonProperty("_key")]
        public string Key { get; set; }

        public Player Player { get; set; }

        public string Type { get; set; }

        public PendingMember(Player player, string type = "request", string key = null)
        {
            this.Player = player;
            this.Type = type;
            this.Key = key;
        } 
    }

    public class Reference
    {
        [JsonProperty("_ref")]
        public string Ref { get; set; }
    }
}