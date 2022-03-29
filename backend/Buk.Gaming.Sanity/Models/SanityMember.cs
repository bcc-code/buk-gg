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
        public SanityMember(Player player, string role)
        {
            SanityType = "member";
            SanityKey = Guid.NewGuid().ToString();
            this.Player = new SanityReference<Player>()
            {
                Ref = player.Id,
            };
            this.Id = player.Id;
            this.Role = role;
        }

        [JsonProperty("_key")]
        public string Id { get; set; }

        public string Role { get; set; }

        [Include]
        public SanityReference<Player> Player { get; set; }

        public int RoleStrength()
        {
            if (this.Role == "owner") return 4;
            if (this.Role == "officer") return 3;
            if (this.Role == "captain") return 2;
            if (this.Role == "member") return 1;
            return 0;
        }

        public Member ToMember() {
            return new Member(this.Player.Value, this.Role, this.SanityKey);
        }
    }
}
