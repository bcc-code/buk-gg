using Buk.Gaming.Models;
using Sanity.Linq;
using Sanity.Linq.BlockContent;
using Sanity.Linq.CommonTypes;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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

        [Include]
        public List<SanityMember> Members { get; set; }

        [Include]
        public List<SanityPendingMember> Pending { get; set; }

        [Include]
        public SanityImage Image { get; set; }

        public Organization ToOrganization () {

            var members = new Member[this.Members != null ? this.Members.Count : 0];

            for (var i = 0; i < members.Length; i++)
            {
                members[i] = this.Members[i].ToMember();
            }
            
            var pending = new PendingMember[this.Pending != null ? this.Pending.Count : 0];

            for (var i = 0; i < pending.Length; i++)
            {
                pending[i] = this.Pending[i].ToPendingMember();
            }

            return new Organization {
                Id = this.Id,
                Image = this.Image?.Asset?.Value?.Url,
                Name = this.Name,
                Members = members,
                Pending = pending,
                IsPublic = this.IsPublic,
            };
        }

        public SanityMember AddMember(User requester, Player player) 
        {
            SanityPendingMember pendingMember = null;
            if (this.Pending != null)
            {
                pendingMember = this.Pending.Find(p => p.Player.Ref == player.Id);
            }

            int roleStrength = pendingMember != null ? 2 : 3;

            if (this?.Members?.Find(m => m.Player.Ref == requester.Id)?.RoleStrength() >= roleStrength)
                {
                    if (this.Members.Find(m => m.Player.Ref == player.Id) == null)
                    {
        
                        this.Members.Add(new SanityMember(player, "member"));

                        if (pendingMember != null)
                        {
                            this.Pending.Remove(pendingMember);
                        }
                        return new SanityMember(player, "member");
                    }
                }
            return null;
        }

        public Member UpdateMember(User requester, Member member) 
        {
            if (this.Members?.Find(m => m.Player.Ref == requester.Id)?.RoleStrength() >= 3)
            {
                var sMember = this.Members.Find(m => m.Player.Ref == member.Player.Id);

                sMember.Role = member.Role;

                return member;
            }
            return null;
        }

        public bool RemoveMember(User requester, string memberId)
        {
            
            if (this.Members?.Find(m => m.Player.Ref == requester.Id)?.RoleStrength() >= 3 || requester.Id == memberId)
            {
                SanityMember member = this.Members?.Find(m => m.Player.Ref == memberId);
                if (member != null) {
                    this.Members.Remove(member);
                }
                //sOrganization.Members = sOrganization.Members.Where(m => m.Player.Ref != memberId).ToList();
                return true;
            }
            return false;
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