using Buk.Gaming.Classes;
using Buk.Gaming.Models;
using Buk.Gaming.Providers;
using Buk.Gaming.Repositories;
using Buk.Gaming.Services;
using Buk.Gaming.Web.Classes;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Buk.Gaming.Web.Services
{
    public class TeamService : BaseService, ITeamService
    {
        private readonly ITeamRepository _teams;
        private readonly IOrganizationService _organizations;
        private readonly IPlayerService _players;

        public TeamService(IMemoryCache cache, ISessionProvider session, ITeamRepository teams, IOrganizationService organizations, IPlayerService players): base(cache, session)
        {
            _teams = teams;
            _organizations = organizations;
            _players = players;
        }

        public Task<Dictionary<string, Team>> GetTeamsAsync()
        {
            return Cache.WithSemaphoreAsync("TEAMS", async () =>
            {
                return (await _teams.GetTeamsAsync()).ToDictionary(t => t.Id, t => t);
            });
        }

        public async Task<Team> CreateTeamAsync(Team.CreateOptions options)
        {
            var user = await Session.GetCurrentUser();

            var organization = await _organizations.GetOrganizationAsync(options.OrganizationId);

            if (!organization.Members.Any(m => m.PlayerId == user.Id && m.Role.Strength >= Role.Officer.Strength))
            {
                throw new Exception("User can't do this");
            }

            Team team = new()
            {
                GameId = options.GameId,
                OrganizationId = options.OrganizationId,
                Members = new()
                {
                    new()
                    {
                        Role = Role.Captain,
                        PlayerId = options.CaptainId,
                    }
                },
                Name = options.Name,
            };

            await _teams.SaveTeamAsync(team);

            var teams = await GetTeamsAsync();

            teams[team.Id] = team;

            return team;
        }

        public async Task<Team> GetTeamAsync(string teamId)
        {
            return (await GetTeamsAsync()).GetValueOrDefault(teamId) ?? throw new Exception("Team not found");
        }

        public async Task<List<Team>> GetTeamsInOrganizationAsync(string organizationId)
        {
            return (await GetTeamsAsync()).Values.Where(i => i.OrganizationId == organizationId).ToList();
        }

        public async Task<List<Team>> GetTeamsInGameAsync(string gameId)
        {
            return (await GetTeamsAsync()).Values.Where(i => i.GameId == gameId).ToList();
        }

        public async Task<Dictionary<string, Player>> GetPlayersAsync(string teamId)
        {
            var team = await GetTeamAsync(teamId);
            return (await _players.GetPlayersAsync()).Where(i => team.Members.Any(m => m.PlayerId == i.Key)).ToDictionary(p => p.Key, p => p.Value);
        }

        public async Task AddPlayersAsync(string teamId, IEnumerable<string> playerIds)
        {
            var user = await Session.GetCurrentUser();
            var team = await GetTeamAsync(teamId);

            bool isCaptain = team.Members.Any(m => m.PlayerId == user.Id && m.Role.Equals(Role.Captain));
            var org = await _organizations.GetOrganizationAsync(team.OrganizationId);

            if (!(isCaptain || org.Members.Any(m => m.PlayerId == user.Id && m.Role.Strength >= Role.Officer.Strength)))
            {
                throw new Exception("User can't do this");
            }

            foreach (var playerId in playerIds)
            {
                var member = org.Members.FirstOrDefault(m => m.PlayerId == playerId);

                if (member == null)
                {
                    throw new Exception("Player must be in organization");
                }

                team.Members.Add(new()
                {
                    PlayerId = playerId,
                    Role = Role.Member
                });
            }

            await _teams.SaveTeamAsync(team);
        }

        public async Task RemovePlayersAsync(string teamId, IEnumerable<string> playerIds)
        {
            var user = await Session.GetCurrentUser();
            var team = await GetTeamAsync(teamId);

            bool isCaptain = team.Members.FirstOrDefault(m => m.PlayerId == user.Id)?.Role.Equals(Role.Captain) ?? false;
            var org = await _organizations.GetOrganizationAsync(team.OrganizationId);

            if (!(isCaptain || org.Members.FirstOrDefault(m => m.PlayerId == user.Id)?.Role.Strength < Role.Officer.Strength))
            {
                throw new Exception("User can't do this");
            }
            
            foreach (var playerId in playerIds)
            {
                var member = team.Members.FirstOrDefault(m => m.PlayerId == playerId);

                if (member == null)
                {
                    throw new Exception("Player not in team");
                }

                team.Members.Remove(member);
            }

            await _teams.SaveTeamAsync(team);
        }

        public async Task SetCaptainAsync(string teamId, string playerId)
        {
            var user = await Session.GetCurrentUser();
            var team = await GetTeamAsync(teamId);
            var org = await _organizations.GetOrganizationAsync(team.OrganizationId);

            if (org.Members.FirstOrDefault(m => m.PlayerId == user.Id)?.Role.Strength < Role.Officer.Strength)
            {
                throw new Exception("User can't do this");
            }

            var captain = team.Members.FirstOrDefault(m => m.Role.Equals(Role.Captain));
            var member = team.Members.FirstOrDefault(m => m.PlayerId == playerId);

            if (captain.PlayerId == member.PlayerId)
            {
                throw new Exception("User is already captain");
            }

            if (member == null)
            {
                if (captain == null)
                {
                    team.Members.Add(new()
                    {
                        PlayerId = playerId,
                        Role = Role.Captain,
                    });
                } else
                {
                    team.Members.Add(new()
                    {
                        PlayerId = captain.PlayerId,
                        Role = Role.Member,
                    });
                    captain.PlayerId = playerId;
                }
            } else
            {
                if (captain != null)
                {
                    captain.Role = Role.Member;
                }
                member.Role = Role.Captain;
            }

            await _teams.SaveTeamAsync(team);
        }

        public async Task UpdateTeamAsync(string teamId, Team.UpdateOptions options)
        {
            var user = await Session.GetCurrentUser();
            var team = await GetTeamAsync(teamId);

            bool isCaptain = team.Members.Any(m => m.PlayerId == user.Id && m.Role.Equals(Role.Captain));
            var org = await _organizations.GetOrganizationAsync(team.OrganizationId);

            if (!(isCaptain || org.Members.Any(m => m.PlayerId == user.Id && m.Role.Strength >= Role.Officer.Strength)))
            {
                throw new Exception("User can't do this");
            }

            if (options.CaptainId != null)
            {
                await SetCaptainAsync(teamId, options.CaptainId);
            }
            if (options.Name != null)
            {
                if (team.Name != options.Name)
                {
                    team.Name = options.Name;
                }
            }

            await _teams.SaveTeamAsync(team);
        }
    }
}
