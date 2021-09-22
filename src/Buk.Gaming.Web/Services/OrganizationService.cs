using Buk.Gaming.Classes;
using Buk.Gaming.Models;
using Buk.Gaming.Providers;
using Buk.Gaming.Repositories;
using Buk.Gaming.Services;
using Buk.Gaming.Web.Classes;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Buk.Gaming.Web.Services
{
    public class OrganizationService : BaseService, IOrganizationService
    {
        private readonly IOrganizationRepository _organizations;
        private readonly IPlayerService _players;

        public OrganizationService(IMemoryCache cache, ISessionProvider session, IOrganizationRepository organizations, IPlayerService players): base(cache, session)
        {
            _organizations = organizations;
            _players = players;
        }

        private Task<Dictionary<string, Organization>> GetAllOrganizationsAsync()
        {
            return Cache.WithSemaphoreAsync("ORGANIZATIONS", async () =>
            {
                var orgs = await _organizations.GetOrganizationsAsync();

                return orgs.ToDictionary(o => o.Id, o => o);
            }, TimeSpan.FromMinutes(30));
        }

        public async Task<List<Organization>> GetOrganizationsAsync(bool includePublic)
        {
            var user = await Session.GetCurrentUser();
            var orgs = await GetAllOrganizationsAsync();

            return orgs.Values.Where(i => (includePublic && i.IsPublic) || i.Members.Any(m => m.PlayerId == user.Id)).ToList();
        }

        public async Task<List<Player>> GetPlayersAsync(string organizationId)
        {
            var org = await GetOrganizationAsync(organizationId);

            var players = await _players.GetPlayersAsync();

            return org.Members.Select(p => players.GetValueOrDefault(p.PlayerId)).Where(i => i != null).ToList();
        }

        public async Task<Organization> GetOrganizationAsync(string id) => (await GetAllOrganizationsAsync()).GetValueOrDefault(id) ?? throw new Exception("Organization not found");

        public async Task AcceptRequestAsync(string organizationId, string playerId)
        {
            var user = await Session.GetCurrentUser();
            if (user == null)
            {
                throw new Exception("User not found");
            }

            var player = await _players.GetPlayerAsync(playerId);
            if (player == null)
            {
                throw new Exception("Player not found");
            }

            var org = await GetOrganizationAsync(organizationId);

            var invitation = org.Invitations.FirstOrDefault(i => i.PlayerId == playerId);

            if (playerId == user.Id && invitation?.Type != InvitationType.Invitation)
            {
                throw new Exception("User can't do this");
            }

            var member = org.Members.FirstOrDefault(m => m.PlayerId == user.Id);

            if (member.Role.Strength < 2)
            {
                throw new Exception("User has no access");
            }

            if (!org.Members.Any(m => m.PlayerId == playerId))
            {
                org.Members.Add(new()
                {
                    PlayerId = player.Id,
                    Role = Role.Member,
                });
            }

            org.Invitations.Remove(invitation);

            await _organizations.SaveOrganizationAsync(org);
        }

        public async Task EditMembersAsync(string organizationId, MemberList.UpdateOptions options)
        {
            var (member, org) = await GetOrganizationWithAccessAndMemberAsync(organizationId, 2);

            if (options.RemoveIds != null)
            {
                foreach (var id in options.RemoveIds)
                {
                    var i = org.Members.Get(id);

                    if (i.Role.Strength >= member.Role.Strength)
                    {
                        throw new Exception("Can't edit users with higher rolestrength");
                    }

                    org.Members.Remove(i);
                }
            }
            if (options.AddIds != null)
            {
                foreach (var id in options.AddIds)
                {
                    var player = await _players.GetPlayerAsync(id);

                    org.Members.AddMember(player.Id);
                }
            }
            if (options.RoleAssignments != null)
            {
                foreach (var entry in options.RoleAssignments)
                {
                    var role = Role.Validate(entry.Value);
                    role.VerifyLessThan(member.Role);

                    org.Members.SetRole(entry.Key, role, role == Role.Owner);
                }
            }

            await _organizations.SaveOrganizationAsync(org);
        }

        public async Task AskToJoinAsync(string organizationId)
        {
            var user = await Session.GetCurrentUser();
            if (user == null)
            {
                throw new Exception("User not found");
            }

            var org = await GetOrganizationAsync(organizationId);

            var member = org.Members.FirstOrDefault(m => m.PlayerId == user.Id);

            if (member != null)
            {
                throw new Exception("Why?");
            }

            org.Invitations.Add(new()
            {
                PlayerId = user.Id,
                Type = InvitationType.Request,
            });

            await _organizations.SaveOrganizationAsync(org);
        }

        public async Task<Organization> CreateOrganizationAsync(Organization.CreateOptions options)
        {
            var user = await Session.GetCurrentUser();

            Organization org = (await GetAllOrganizationsAsync()).Values.FirstOrDefault(i => i.Members.Any(m => m.PlayerId == user.Id && m.Role.Equals(Role.Owner)));

            if (org != null)
            {
                return org;
            }

            org = new()
            {
                IsPublic = false,
                Members = new()
                {
                    new()
                    {
                        Role = Role.Owner,
                        PlayerId = user.Id,
                    }
                },
                Name = options.Name,
            };

            await _organizations.SaveOrganizationAsync(org);

            return org;
        }

        public async Task UpdateOrganizationAsync(string id, Organization.UpdateOptions options)
        {
            var org = await GetOrganizationWithAccessAsync(id);

            if (options.Name != null)
            {
                org.Name = options.Name;
            }

            if (options.Image != null)
            {
                byte[] bytes = Convert.FromBase64String(options.Image);

                MemoryStream ms = new(bytes);

                await _organizations.SetImageAsync(org, ms);
            }
            else
            {
                await _organizations.SaveOrganizationAsync(org);
            }
        }

        public async Task SetOrganizationImageAsync(string organizationId, Stream image)
        {
            var org = await GetOrganizationWithAccessAsync(organizationId);

            await _organizations.SetImageAsync(org, image);
        }

        public Task DeleteOrganizationAsync(string id)
        {
            throw new NotImplementedException();
        }

        private async Task<(Member Member, Organization Organization)> GetOrganizationWithAccessAndMemberAsync(string organizationId, int strength = 3)
        {
            var user = await Session.GetCurrentUser();

            Organization org = await GetOrganizationAsync(organizationId);

            var member = org.Members.FirstOrDefault(p => p.PlayerId == user.Id);

            if (member?.Role.Strength < strength)
            {
                throw new Exception("No access");
            }

            return new(member, org);
        }

        private async Task<Organization> GetOrganizationWithAccessAsync(string organizationId, int strength = 3)
        {
            return (await GetOrganizationWithAccessAndMemberAsync(organizationId, strength)).Organization;
        }
    }
}
