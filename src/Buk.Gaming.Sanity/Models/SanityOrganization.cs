using Buk.Gaming.Models;
using Sanity.Linq;
using Sanity.Linq.BlockContent;
using Sanity.Linq.CommonTypes;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Buk.Gaming.Sanity.Models {
    public class SanityOrganization : SanityDocument
    {
        public SanityOrganization()
        {
            SanityType = "organization";
        }

        public string Name { get; set; }

        public bool IsPublic { get; set; }

        public string DiscordRoleId { get; set; }

        public List<SanityMember> Members { get; set; } = new List<SanityMember>();

        public List<SanityPendingMember> Pending { get; set; } = new List<SanityPendingMember>();

        [Include]
        public SanityImage Image { get; set; }

        public bool HasEditAccess(User user)
        {
            return Members.Where(m => m.Player.Ref == user.Id).Select(m => m.RoleStrength()).FirstOrDefault() >= 3;
        }

        public Organization ToOrganization () {
            return new Organization {
                Id = Id,
                Image = Image?.Asset?.Value?.Url,
                Name = Name,
                Members = Members.Select(m => m.ToMember()).ToList(),
                Pending = Pending.Select(p => p.ToPendingMember()).ToList(),
                IsPublic = IsPublic,
            };
        }
    }

    public class SanityPendingMember
    {
        [JsonProperty("_key")]
        public string Key { get; set; }

        [Include]
        public SanityReference<Player> Player { get; set; }
        
        public string Type { get; set; }

        public SanityPendingMember(Player player, string type = "request")
        {
            this.Player = new SanityReference<Player> {
                Ref = player.Id,
            };

            this.Key = player.Id;

            this.Type = type;
        }

        public PendingMember ToPendingMember() {
            return new PendingMember(this.Player.Value, this.Type, this.Key);
        }
    }
}