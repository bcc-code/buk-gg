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

namespace Buk.Gaming.Sanity {
    public class SanityTeamRepository : ITeamRepository {

        public SanityTeamRepository(SanityDataContext sanity, IMemoryCache cache)
        {
            Sanity = sanity;
            Cache = cache;
        }

        public SanityDataContext Sanity { get; }

        public IMemoryCache Cache { get; }

        private readonly string PlayerQuery = "{_id, nickname, discordUser, discordId, email}";

        private string MemberQuery {
            get {
                return $"'members': members[]{{..., player->{this.PlayerQuery}}}";
            }
        }
        private string TeamQuery => $"{{..., captain->{this.PlayerQuery}, organization->{{..., 'image': image.asset->url, {this.MemberQuery}}}, game->{{..., 'icon': icon.asset->url}}, 'players': players[]->{this.PlayerQuery}}}";

        public async Task<Team> GetTeamAsync(string teamId) 
        {
            return (await GetTeamsAsync()).FirstOrDefault(t => t.Id == teamId);
        }

        public Task<List<Team>> GetTeamsAsync()
        {
            return Cache.GetOrCreateAsync("Sanity_Teams", async (c) =>
            {
                c.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60);
                string query = $"*[_type == 'team' && !(_id in path('drafts.**'))]{this.TeamQuery}";
                var result = await Sanity.Client.FetchAsync<List<Team>>(query);
                return result.Result;
            });
        }

        public async Task<List<Team>> GetMyTeamsAsync(string playerId) 
        {
            List<Team> teams = (await GetTeamsAsync()).Where(t => t.Captain.Id == playerId || t.Players.FirstOrDefault(p => p.Id == playerId) != null).ToList();

            return teams;
        }

        public async Task<List<Team>> GetTeamsInGameAsync(string gameId)
        {
            List<Team> teams = (await GetTeamsAsync()).Where(t => t.Game.Id == gameId).ToList();

            return teams;
        }

        public async Task<Team> AddTeamAsync(User requester, Team team)
        {
            if (string.IsNullOrEmpty(team.Id))
            {
                team.Id = Guid.NewGuid().ToString();

                SanityOrganization org = await Sanity.DocumentSet<SanityOrganization>().GetAsync(team.Organization?.Id);

                SanityMember mem = org?.Members.Find(m => m.Player.Ref == requester.Id);

                if (mem?.Role == "owner" || mem?.Role == "officer")
                {
                    SanityTeam sTeam = new SanityTeam() {
                        Id = team.Id,
                        Name = team.Name,
                        Captain = new SanityReference<Player>() {
                            Ref = team.Captain.Id,
                        },
                        Game = new SanityReference<SanityGame>() {
                            Ref = team.Game.Id,
                        },
                        Organization = new SanityReference<SanityOrganization>() {
                            Ref = team.Organization.Id
                        }
                    };

                    await Sanity.DocumentSet<SanityTeam>().Create(sTeam).CommitAsync();

                    (await GetTeamsAsync()).Add(team);

                    return team;
                }
            }
            return null;
        }

        public class TournamentParticipant {
            public bool IsParticipant { get; }

            public int MinPlayers { get; }
        }

        public async Task<SanityResult<Team>> UpdateTeamAsync(User requester, Team team)
        {
            SanityTeam sTeam = await Sanity.DocumentSet<SanityTeam>().GetAsync(team.Id);

            if (sTeam == null) return new SanityResult<Team>{
                Item = null,
                Success = false,
                Reason = "notFound"
            };

            Team cachedTeam = (await GetTeamsAsync()).FirstOrDefault(t => t.Id == team.Id);

            if (sTeam.Captain?.Ref == requester.Id)
            {
                List<SanityReference<Player>> pList = new List<SanityReference<Player>>();

                foreach(Player player in team.Players) {
                    if (player.Id != team.Captain.Id && pList.Find(p => p.Ref == player.Id) == null) {
                        pList.Add(new SanityReference<Player>() {
                            Ref = player.Id,
                        });
                    }
                }

                sTeam.Name = team.Name;
                cachedTeam.Name = team.Name;

                cachedTeam.Captain = team.Captain;
                sTeam.Captain = new SanityReference<Player>() {
                    Ref = team.Captain.Id,
                };

                cachedTeam.Players = team.Players;
                sTeam.Players = pList;

                var teamSizes = await Sanity.Client.FetchAsync<List<int>>($"*[_type == 'tournament' && count(teams) > 0 && '{team.Id}' in teams[].team->_id].teamSize.min");

                foreach(int teamSize in teamSizes.Result)
                {
                    if (teamSize > (team.Players.Count + 1)) {
                        return new SanityResult<Team>{
                            Item = null,
                            Success = false,
                            Reason = "signedUpForTournament"
                        };
                    }
                }
                await Sanity.DocumentSet<SanityTeam>().Update(sTeam).CommitAsync();

                return new SanityResult<Team>{
                    Item = team,
                    Success = true
                };
            }

            // CHECKING IF PLAYER HAS PERMISSIONS IN ORGANIZATION
            SanityOrganization sOrganization = await Sanity.DocumentSet<SanityOrganization>().GetAsync(sTeam.Organization.Ref);

            if (sOrganization?.Members?.Find(m => m.Player.Ref == requester.Id)?.RoleStrength() >= 3)
            {
                List<SanityReference<Player>> pList = new List<SanityReference<Player>>();

                foreach (Player player in team.Players)
                {
                    pList.Add(new SanityReference<Player>()
                    {
                        Ref = player.Id,
                    });
                }

                sTeam.Name = team.Name;

                sTeam.Captain = new SanityReference<Player>()
                {
                    Ref = team.Captain.Id,
                };

                sTeam.Players = pList;

                var teamSizes = await Sanity.Client.FetchAsync<List<int>>($"*[_type == 'tournament' && count(teams) > 0 && '{team.Id}' in teams[].team->_id].teamSize.min"); 

                foreach(int teamSize in teamSizes.Result)
                {
                    if (teamSize > (team.Players.Count + 1)) {
                        return new SanityResult<Team>{
                            Item = null,
                            Success = false,
                            Reason = "signedUpForTournament"
                        };
                    }
                }
                await Sanity.DocumentSet<SanityTeam>().Update(sTeam).CommitAsync();

                return new SanityResult<Team>{
                    Item = team,
                    Success = true
                };
            }
            return new SanityResult<Team>{
                Item = null,
                Success = false,
                Reason = "noPermissions"
            };
        }

        public async Task<bool> DeleteTeamAsync(User requester, Team team)
        {
            SanityOrganization sOrganization = await Sanity.DocumentSet<SanityOrganization>().GetAsync(team.Organization.Id);
            
            if (sOrganization?.Members?.Find(m => m.Player.Ref == requester.Id)?.RoleStrength() >= 3)
            {
                // var references = await Sanity.Client.FetchAsync<List<Object>>($"*[references({team.Id}]");
                // if (references.Result?.Count > 0) return false;
                await Sanity.DocumentSet<SanityTeam>().DeleteById(team.Id).CommitAsync();

                return true;
            }
            return false;
        }

        public Task<List<Game>> GetGamesAsync()
        {
            var lang = System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
            return Cache.GetOrCreateAsync("Sanity_Games_" + lang, async (c) =>
            {
                c.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60);

                var result = await Sanity.DocumentSet<SanityGame>().ToListAsync();

                return result.Select(g => g.ToGame()).ToList();
            });
        }
    }
}