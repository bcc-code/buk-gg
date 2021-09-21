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

        public TeamService(IMemoryCache cache, ISessionProvider session, ITeamRepository teams, IOrganizationService organizations): base(cache, session)
        {
            _teams = teams;
            _organizations = organizations;
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

            await _teams.SaveOrCreateTeamAsync(team);

            return team;
        }

        public Task<Team> GetTeamAsync(string teamId)
        {
            return Cache.WithSemaphoreAsync("TEAM_" + teamId, async () =>
            {
                return await _teams.GetTeamAsync(teamId);
            }, TimeSpan.FromSeconds(5));
        }

        public Task<List<Team>> GetTeamsInOrganizationAsync(string organizationId)
        {
            return Cache.WithSemaphoreAsync("TEAMS_ORGANIZATION_" + organizationId, async () =>
            {
                return await _teams.GetTeamsForOrganizationAsync(organizationId);
            }, TimeSpan.FromMinutes(1));
        }

        public Task<List<Team>> GetTeamsInGameAsync(string gameId)
        {
            return Cache.WithSemaphoreAsync("TEAMS_GAME_" + gameId, async () =>
            {
                return await _teams.GetTeamsForGameAsync(gameId);
            }, TimeSpan.FromMinutes(1));
        }

        public Task<List<Team>> GetTeamsInTournamentAsync(string tournamentId)
        {
            return Cache.WithSemaphoreAsync("TEAMS_TOURNAMENT_" + tournamentId, () => _teams.GetTeamsForTournamentAsync(tournamentId));
        }

        public async Task AddPlayerAsync(string teamId, string playerId)
        {
            var user = await Session.GetCurrentUser();
            var team = await GetTeamAsync(teamId);

            bool isCaptain = team.Members.Any(m => m.PlayerId == user.Id && m.Role.Equals(Role.Captain));
            var org = await _organizations.GetOrganizationAsync(team.OrganizationId);

            if (!(isCaptain || org.Members.Any(m => m.PlayerId == user.Id && m.Role.Strength >= Role.Officer.Strength)))
            {
                throw new Exception("User can't do this");
            }

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

            await _teams.SaveOrCreateTeamAsync(team);
        }

        public async Task RemovePlayerAsync(string teamId, string playerId)
        {
            var user = await Session.GetCurrentUser();
            var team = await GetTeamAsync(teamId);

            bool isCaptain = team.Members.FirstOrDefault(m => m.PlayerId == user.Id)?.Role.Equals(Role.Captain) ?? false;
            var org = await _organizations.GetOrganizationAsync(team.OrganizationId);

            if (!(isCaptain || org.Members.FirstOrDefault(m => m.PlayerId == user.Id)?.Role.Strength < Role.Officer.Strength))
            {
                throw new Exception("User can't do this");
            }

            var member = team.Members.FirstOrDefault(m => m.PlayerId == playerId);

            if (member == null)
            {
                throw new Exception("Player not in team");
            }

            team.Members.Remove(member);

            await _teams.SaveOrCreateTeamAsync(team);
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

            await _teams.SaveOrCreateTeamAsync(team);
        }
    }
}
