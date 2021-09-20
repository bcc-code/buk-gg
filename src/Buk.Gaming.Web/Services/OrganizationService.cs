using Buk.Gaming.Classes;
using Buk.Gaming.Models;
using Buk.Gaming.Providers;
using Buk.Gaming.Repositories;
using Buk.Gaming.Services;
using Buk.Gaming.Web.Classes;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Buk.Gaming.Web.Services
{
    public class OrganizationService : BaseService, IOrganizationService
    {
        private readonly IOrganizationRepository _organizations;
        private readonly IPlayerRepository _players;

        public OrganizationService(IMemoryCache cache, ISessionProvider session, IOrganizationRepository organizations, IPlayerRepository players): base(cache, session)
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

        public async Task AddMemberAsync(string organizationId, string playerId)
        {
            var user = await Session.GetCurrentUser();
            if (user == null)
            {
                throw new Exception("User not found");
            }

            if (playerId == user.Id)
            {
                throw new Exception("User can't do this");
            }

            var player = await _players.GetPlayerAsync(playerId);
            if (player == null)
            {
                throw new Exception("Player not found");
            }

            var org = await GetOrganizationAsync(organizationId);

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

        public Task DeleteOrganizationAsync(string id)
        {
            throw new NotImplementedException();
        }
    }
}
