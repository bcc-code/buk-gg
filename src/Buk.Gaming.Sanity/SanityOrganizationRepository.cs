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

        private int GetPermissionStrength(string role) 
        {
            if (role == "owner") return 4;
            if (role == "officer") return 3;
            if (role == "captain") return 2;
            if (role == "member") return 1;
            return 0;
        }

        public async Task<List<Organization>> GetAllOrganizationsAsync()
        {
            return (await GetOrganizationsAsync()).Select(o => o.ToOrganization()).ToList();
        }

        private Task<List<SanityOrganization>> GetOrganizationsAsync()
        {
            return Cache.GetOrCreateAsync("ORGANIZATIONS", async (c) =>
            {
                c.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

                var result = await Sanity.DocumentSet<SanityOrganization>().ToListAsync();

                return result;
            });
        }

        public async Task<Organization> SaveOrganizationAsync(Organization organization, Player player)
        {
            if (!string.IsNullOrEmpty(organization.Id)) {
                SanityOrganization org = (await GetOrganizationsAsync()).FirstOrDefault(o => o.Id == organization.Id);

                var member = org.Members.Find(m => m.Player.Ref == player.Id);

                if (member == null || this.GetPermissionStrength(member.Role) < 3)
                {
                    throw new Exception("User does not have access");
                }

                org.Name = organization.Name;

                await Sanity.DocumentSet<SanityOrganization>().Update(org).CommitAsync();
                
                return org.ToOrganization();
            }
            throw new Exception("Organization Id is not valid");
        }

        public async Task<Organization> CreateOrganizationAsync(Organization organization, Player player)
        {
            List<SanityOrganization> organizations = await GetOrganizationsAsync();
            SanityOrganization existingOrganization = organizations.FirstOrDefault(o => o.Members.Find(m => m.Player.Ref == player.Id && m.Role == "owner") != null);

            if (existingOrganization != null) return existingOrganization.ToOrganization();
            
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

                organizations.Add(newOrg);

                return newOrg.ToOrganization();
            }
            return organization;
        }

        public async Task<List<Organization>> GetPlayerOrganizationsAsync(Player player, string role = "")
        {
            List<SanityOrganization> organizations = await GetOrganizationsAsync();

            return organizations.Where(o => o.Members.FirstOrDefault(m => m.Player.Ref == player.Id) != null).Select(o => o.ToOrganization()).ToList();
        }

        public async Task<List<Player>> SearchForPlayersAsync(User requester, string searchString)
        {
            if (searchString.Length < 2) return null;

            string query = $"*[_type == 'player' && discordIsConnected && ((email == '{searchString}') || (location == '{requester.Location}' && (name match '*{searchString}*' || nickname match '*{searchString}*' || discordUser match '*{searchString}*')))]";

            var searchResult = await Sanity.Client.FetchAsync<List<Player>>(query);

            return searchResult.Result.ToList();
        }

        // MEMBERS
        public async Task<Organization> AddMemberAsync(User requester, string organizationId, Player player)
        {
            List<SanityOrganization> organizations = await GetOrganizationsAsync();

            SanityOrganization organization = organizations.FirstOrDefault(o => o.Id == organizationId);

            if (organization == null)
            {
                throw new Exception("Organization not found");
            }

            organization.Members.Add(new SanityMember(player, "member"));

            await Sanity.DocumentSet<SanityOrganization>().Update(organization).CommitAsync();

            return organization.ToOrganization();
        }

        public async Task<Organization> UpdateMemberAsync(User requester, string organizationId, Member member)
        {
            SanityOrganization organization = await Sanity.DocumentSet<SanityOrganization>().GetAsync(organizationId);

            SanityMember m = organization.Members.FirstOrDefault(m => m.Player.Ref == member?.Player.Id);


            if (m != null)
            {
                m.Role = member.Role;

                await Sanity.DocumentSet<SanityOrganization>().Update(organization).CommitAsync();
                
                return organization.ToOrganization();
            }
            return null;
        }

        public async Task<Organization> DeleteMemberAsync (User requester, string organizationId, string memberId)
        {
            SanityOrganization organization = (await GetOrganizationsAsync()).FirstOrDefault(o => o.Id == organizationId && o.Members.Where(m => m.RoleStrength() >= 3 && m.Player.Ref == requester.Id).ToList().Count > 0);

            SanityMember member = organization?.Members.FirstOrDefault(m => m.Player.Ref == memberId);

            if (member != null)
            {
                organization.Members.Remove(member);

                await Sanity.DocumentSet<SanityOrganization>().Update(organization).CommitAsync();
                
                return organization.ToOrganization();
            }
            throw new Exception("Member not found or user has no access");
        }

        public async Task<Organization> AddPendingMember (User requester, string organizationId, Player player, string type)
        {
            SanityOrganization organization = (await GetOrganizationsAsync()).FirstOrDefault(o => o.Id == organizationId);

            if (organization != null && organization.Members != null && organization.Members.Find(m => m.Player.Ref == player.Id) == null)
            {
                organization.Pending.Add(new SanityPendingMember(player, "request"));

                await Sanity.DocumentSet<SanityOrganization>().Update(organization).CommitAsync();

                return organization.ToOrganization();
            }
            throw new Exception();
        }

        public async Task<Organization> AcceptPendingMember (User requester, string organizationId, string playerId)
        {
            SanityOrganization organization = (await GetOrganizationsAsync()).FirstOrDefault(o => o.Id == organizationId && o.HasEditAccess(requester));

            if (organization == null)
            {
                throw new Exception();
            }

            SanityPendingMember member = organization.Pending.Find(m => m.Player.Ref == playerId);

            if (member == null)
            {
                throw new Exception();
            }

            organization.Members.Add(new SanityMember(member.Player.Value, "member"));

            organization.Pending.Remove(member);

            await Sanity.DocumentSet<SanityOrganization>().Update(organization).CommitAsync();

            return organization.ToOrganization();
        }

        public async Task<Organization> RemovePendingMember (User requester, string organizationId, string playerId)
        {
            SanityOrganization organization = (await GetOrganizationsAsync()).FirstOrDefault(o => o.Id == organizationId);

            SanityPendingMember member = organization.Pending.Find(m => m.Player.Ref == playerId);

            if (member != null && organization != null && (member?.Player.Ref == requester.Id || organization.HasEditAccess(requester)))
            {
                organization.Pending.Remove(member);

                await Sanity.DocumentSet<SanityOrganization>().Update(organization).CommitAsync();

                return organization.ToOrganization();
            }
            throw new Exception();
        }

        // IMAGE
        public async Task<Organization> UpdateImageAsync(User requester, string organizationId, System.IO.Stream image)
        {

            SanityOrganization organization = (await GetOrganizationsAsync()).FirstOrDefault(o => o.Id == organizationId && o.HasEditAccess(requester));
            if (organization != null)
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

                return organization.ToOrganization();
            }
            throw new Exception();
        }

    }
}
