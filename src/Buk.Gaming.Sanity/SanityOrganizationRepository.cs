using Buk.Gaming.Models;
using Buk.Gaming.Repositories;
using Buk.Gaming.Sanity.Models;
using Microsoft.Extensions.Caching.Memory;
using Sanity.Linq;
using Sanity.Linq.Extensions;
using Sanity.Linq.CommonTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buk.Gaming.Sanity
{
    public class SanityOrganizationRepository : IOrganizationRepository
    {
        public SanityOrganizationRepository(SanityDataContext sanity, IMemoryCache cache)
        {
            Sanity = sanity;
            Cache = cache;
        }

        public SanityDataContext Sanity { get; }
        public IMemoryCache Cache { get; }

        private string PlayerQuery = "{_id, nickname, discordUser, discordId, email}";

        private int GetPermissionStrength(string role) 
        {
            if (role == "owner") return 4;
            if (role == "officer") return 3;
            if (role == "captain") return 2;
            if (role == "member") return 1;
            return 0;
        }

        private void UpdateOrganizationCache(Organization organization)
        {
            var orgs = Cache.Get<Organization[]>("Sanity_Organizations");
            if (orgs?.Length > 0) {
                var org = orgs.FirstOrDefault(o => o.Id == organization.Id);
                if (org != null) {
                    org.Name = organization.Name;
                    org.Image = organization.Image;
                    org.Members = organization.Members;
                    org.Pending = organization.Pending;
                } else {
                    orgs.Append<Organization>(organization);
                }

                Cache.Set<Organization[]>("Sanity_Organizations", orgs, TimeSpan.FromSeconds(60));
            }
        }

        // ORGANIZATIONS
        public Task<Organization[]> GetOrganizationsAsync()
        {
            return Cache.GetOrCreateAsync("Sanity_Organizations", async (c) =>
            {
                c.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60);

                // var result = await Sanity.DocumentSet<SanityOrganization>().Where(o => !o.IsDraft()).ToListAsync();

				// return result.Select(o => o.ToOrganization(Sanity.HtmlBuilder)).ToArray();

                string query = $"*[_type == 'organization' && !(_id in path('drafts.**'))]{{...,'image': image.asset->url, 'members': members[]{{player->{this.PlayerQuery}, role}}, 'pending': pending[]{{player->{this.PlayerQuery}, type}}, 'teams': *[_type == 'team' && references(^._id)]{{..., game->{{..., 'icon': icon.asset->url}}, 'members': members[]{{player->{this.PlayerQuery}, role}}}}}}";
                var result = await Sanity.Client.FetchAsync<Organization[]>(query);

                // var result = await Sanity.DocumentSet<SanityOrganization>().Where(o => o.IsDraft() == false).ToListAsync();

                // return result.Select(o => o.ToOrganization()).ToArray();

                return result.Result;
            });
        }

        public async Task<Organization> SaveOrganizationAsync(Organization organization, Player player)
        {
            if (!string.IsNullOrEmpty(organization.Id)) {
                var org = await Sanity.DocumentSet<SanityOrganization>().Where(o => o.Id == organization.Id).FirstOrDefaultAsync();

                var member = org.Members.Find(m => m.Player.Ref == player.Id);

                if (member == null || this.GetPermissionStrength(member.Role) < 3) return new Organization();
    
                var cached = Cache.Get<Organization[]>("Sanity_Organizations").FirstOrDefault(o => o.Id == organization.Id);

                // ALL FIELDS TO UPDATE
                org.Name = organization.Name;

                // List<SanityMember> Members = new List<SanityMember>();

                // for(int i = 0; i < organization.Members.Length; i++)
                // {
                //     Member Member = organization.Members[i];
                //     Members.Add(new SanityMember(Member.Player, Member.Role));
                // }
                
                // org.Members = Members;

                await Sanity.DocumentSet<SanityOrganization>().Update(org).CommitAsync();

                UpdateOrganizationCache(organization);

                cached = Cache.Get<Organization[]>("Sanity_Organizations").FirstOrDefault(o => o.Id == organization.Id);

                string query = $"*[_type == 'organization' && _id == '{org.Id}' && !(_id in path('drafts.**'))][0]{{..., 'image': image.asset->url, 'members': members[]{{player->{this.PlayerQuery}, role}}, 'teams': *[_type == 'team' && references(^._id)]{{..., game->{{..., 'icon': icon.asset->url}}, 'members': members[]{{player->{this.PlayerQuery}, role}}, 'organizationId': ^._id}}}}";
                var result = await Sanity.Client.FetchAsync<Organization>(query);

                // return (await Sanity.DocumentSet<SanityOrganization>().GetAsync(org.Id))?.ToOrganization();
                return result.Result;
            }
            return new Organization();
        }

        public async Task<Organization> CreateOrganizationAsync(Organization organization, Player player)
        {
            string query = $"*[_type == 'organization' && !(_id in path('drafts.**')) && '{player.Id}' in members[role == 'owner'].player._ref]{{...,'image': image.asset->url, 'members': members[]{{player->{this.PlayerQuery}, role}}, 'teams': *[_type == 'team' && references(^._id)]{{..., game->{{..., 'icon': icon.asset->url}}, 'members': members[]{{player->{this.PlayerQuery}, role}}}}}}";
            var oldOrg = await Sanity.Client.FetchAsync<Organization[]>(query);

            if (oldOrg.Result?.FirstOrDefault() != null) return oldOrg.Result.FirstOrDefault();
            
            if (string.IsNullOrEmpty(organization.Id))
            {
                organization.Id = Guid.NewGuid().ToString();
                var newOrg = new SanityOrganization()
                {
                    Name = organization.Name,
                    Id = organization.Id,
                    Members = new List<SanityMember>(),
                };

                newOrg.Members.Add(new SanityMember(player, "owner"));

                await Sanity.DocumentSet<SanityOrganization>().Create(newOrg).CommitAsync();

                organization.Members = new Member[1];
                organization.Members[0] = new Member(player, "owner");

                UpdateOrganizationCache(organization);

                return organization;
            }
            return organization;
        }

        public async Task<Organization[]> GetPlayerOrganizationsAsync(Player player, string role = "")
        {
            Organization[] organizations = await GetOrganizationsAsync();

            return organizations.Where(o => o.Members.FirstOrDefault(m => m.Player.Id == player.Id) != null).ToArray();
        }

        // MEMBERS
        public Task<Member> GetMemberAsync(Player player)
        {
            return Cache.GetOrCreateAsync("Sanity_Player_" + player.Id, async (c) =>
            {
                c.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10);

                string search = !string.IsNullOrEmpty(player.Id) ? $"_id == '{player.Id}'" : !string.IsNullOrEmpty(player.Email) ? $"email == '{player.Email}'" : !string.IsNullOrEmpty(player.DiscordId) ? $"discordId == '{player.DiscordId}'" : "_id == 'noIdHere'";

                string query = $"*[_type == 'player' && {search}][0]{this.PlayerQuery}";
                var result = await Sanity.Client.FetchAsync<Player>(query);
                var member = new Member(result.Result, "member");
                return member;
            });
        }

        public async Task<Player[]> SearchForPlayersAsync(User requester, string searchString)
        {
            if (searchString.Length < 2) return new Player[0];

            string query = $"*[_type == 'player' && discordIsConnected && ((email == '{searchString}') || (location == '{requester.Location}' && (name match '*{searchString}*' || nickname match '*{searchString}*' || discordUser match '*{searchString}*')))]";

            var searchResult = await Sanity.Client.FetchAsync<List<Player>>(query);
            
            // var localPlayers = await Sanity.DocumentSet<Player>().Where(p => p.Location == requester.Location && p.DiscordIsConnected).ToListAsync();
            
            // List<Player> searchedPlayers = new List<Player>();

            // for (int i = 0; i < localPlayers?.Count; i++)
            // {
            //     Player player = localPlayers[i];

            //     if (player.Email == searchString || player.Name.Contains(searchString) || player.DisplayName.Contains(searchString))
            //     {
            //         searchedPlayers.Add(player);
            //     }
            // }

            // if (searchedPlayers.Count > 0)
            // {
            //     return searchedPlayers.ToArray();
            // }

            // var result = await Sanity.DocumentSet<Player>().Where(p => p.Email == searchString && p.DiscordIsConnected).ToListAsync();

            return searchResult.Result.ToArray();
        }

        // MEMBERS
        public async Task<Member> AddMemberAsync(User requester, string organizationId, Player player)
        {
            if (player?.Id != null) 
            {
                SanityOrganization sOrganization = await Sanity.DocumentSet<SanityOrganization>().GetAsync(organizationId);
                var member = sOrganization?.AddMember(requester, player);
                if (member != null) {
                    await Sanity.DocumentSet<SanityOrganization>().Update(sOrganization).CommitAsync();

                    UpdateOrganizationCache(sOrganization.ToOrganization());

                    return member.ToMember();
                }
            }
            return null;
        }

        public async Task<Member> UpdateMemberAsync(User requester, string organizationId, Member member)
        {
            SanityOrganization sOrganization = await Sanity.DocumentSet<SanityOrganization>().GetAsync(organizationId);
            
            var m = sOrganization?.UpdateMember(requester, member);

            if (member != null)
            {
                await Sanity.DocumentSet<SanityOrganization>().Update(sOrganization).CommitAsync();
                
                UpdateOrganizationCache(sOrganization.ToOrganization());
                
                return member;
            }
            return null;
        }

        public async Task<bool> DeleteMemberAsync (User requester, string organizationId, string memberId)
        {
            SanityOrganization sOrganization = await Sanity.DocumentSet<SanityOrganization>().GetAsync(organizationId);

            if (sOrganization?.RemoveMember(requester, memberId) == true)
            {
                await Sanity.DocumentSet<SanityOrganization>().Update(sOrganization).CommitAsync();

                UpdateOrganizationCache(sOrganization.ToOrganization());
                
                return true;
            }
            return false;
        }

        public async Task<bool> AddPendingMember (User requester, string organizationId, Player player, string type)
        {
            SanityOrganization organization = await Sanity.DocumentSet<SanityOrganization>().GetAsync(organizationId);

            if (organization != null && organization.Members != null && organization.Members.Find(m => m.Player.Ref == player.Id) == null)
            {
                if (organization.Pending == null) 
                {
                    organization.Pending = new List<SanityPendingMember>();
                }
                organization.Pending.Add(new SanityPendingMember(player, "request"));

                await Sanity.DocumentSet<SanityOrganization>().Update(organization).CommitAsync();
                
                UpdateOrganizationCache(organization.ToOrganization());

                return true;
            }
            return false;
        }

        public async Task<bool> RemovePendingMember (User requester, string organizationId, string playerId)
        {
            SanityOrganization organization = await Sanity.DocumentSet<SanityOrganization>().GetAsync(organizationId);

            SanityPendingMember member = organization.Pending.Find(m => m.Player.Ref == playerId);

            if (member != null && organization != null && organization.Pending != null && (member?.Player.Ref == requester.Id || organization.Members?.Find(m => m.Player.Ref == requester.Id)?.RoleStrength() >= 3))
            {
                organization.Pending.Remove(member);

                await Sanity.DocumentSet<SanityOrganization>().Update(organization).CommitAsync();

                UpdateOrganizationCache(organization.ToOrganization());

                return true;
            }
            return false;
        }

        // IMAGE
        public async Task<string> UpdateImageAsync(User requester, string organizationId, System.IO.Stream image)
        {

            var organization = await Sanity.DocumentSet<SanityOrganization>().Where(o => o.Id == organizationId).FirstOrDefaultAsync();
            if (this.GetPermissionStrength(organization.Members?.Find(m => m.Player.Ref == requester.Id)?.Role) >= 3)
            {
                var result = await Sanity.Images.UploadAsync(image, organizationId + "_image_" + new DateTime().ToString(), null, organizationId + "_image");

                if (organization.Image == null || organization.Image.Asset == null)
                {
                    organization.Image = new SanityImage() {
                        Asset = new SanityReference<SanityImageAsset>()
                    };
                }

                organization.Image.Asset.Ref = result.Document.Id;

                await Sanity.DocumentSet<SanityOrganization>().Update(organization).CommitAsync();

                UpdateOrganizationCache(organization.ToOrganization());

                return result.Document.Url;
            }
            return "";
        }

    }
}
