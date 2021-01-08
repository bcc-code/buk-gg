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

        private string PlayerQuery = "{_id, nickname, discordUser, discordId, email}";

        private string MemberQuery {
            get {
                return $"'members': members[]{{..., player->{this.PlayerQuery}}}";
            }
        }
        private string TeamQuery {
            get {
                return $"{{..., captain->{this.PlayerQuery}, organization->{{..., 'image': image.asset->url, {this.MemberQuery}}}, game->{{..., 'icon': icon.asset->url}}, 'players': players[]->{this.PlayerQuery}}}"; 
            }
        }

        private void UpdateTeamCache(Team team, bool delete = false)
        {
            var teams = Cache.Get<Team[]>("Sanity_Teams");
            if (teams?.Length > 0) {
                var t = teams.FirstOrDefault(t => t.Id == team.Id);
                if (t != null) {
                    if (delete) {
                        teams = teams.Where(team => team.Id != t.Id).ToArray();
                    } else {
                        t.Name = team.Name;
                        t.Players = team.Players;
                        t.Organization = team.Organization;
                    }
                } else {
                    teams.Append<Team>(team);
                }
                Cache.Set<Team[]>("Sanity_Teams", teams, TimeSpan.FromSeconds(60));
            }
        }

        public Task<Team> GetTeamAsync(string teamId) 
        {
            var lang = System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
            return Cache.GetOrCreateAsync("Sanity_Team_" + teamId + "_" + lang, async (c) =>
            {
                c.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10);
                string query = $"*[_type == 'team' && _id == '{teamId}' && !(_id in path('drafts.**'))][0]{this.TeamQuery}";
                var result = await Sanity.Client.FetchAsync<Team>(query);
                return result.Result;
            });
        }

        public Task<Team[]> GetTeamsAsync()
        {
            return Cache.GetOrCreateAsync("Sanity_Teams", async (c) =>
            {
                c.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60);
                string query = $"*[_type == 'team' && !(_id in path('drafts.**'))]{this.TeamQuery}";
                var result = await Sanity.Client.FetchAsync<Team[]>(query);
                return result.Result;
            });
        }

        public Task<Team[]> GetMyTeamsAsync(string playerId) 
        {
            return Cache.GetOrCreateAsync("Sanity_Teams_" + playerId, async (c) =>
            {
                c.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(20);
                string query = $"*[_type == 'team' && !(_id in path('drafts.**')) && ('{playerId}' == captain._ref || '{playerId}' in players[]._ref || '{playerId}' in organization->members[role in ['owner', 'officer']].player._ref)]{this.TeamQuery}";
                var result = await Sanity.Client.FetchAsync<Team[]>(query);
                return result.Result;
            });
        }

        public Task<Team[]> GetTeamsInGameAsync(string gameId)
        {
            var lang = System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
            return Cache.GetOrCreateAsync("Sanity_Game_" + gameId + "_Teams_" + lang, async (c) =>
            {
                c.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10);
                string query = $"*[_type == 'team' && references('{gameId}') && !(_id in path('drafts.**'))]{this.TeamQuery}";
                var result = await Sanity.Client.FetchAsync<Team[]>(query);
                return result.Result;
            });
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

                    UpdateTeamCache(team);

                    return team;
                } else {
                    return new Team();
                }
            }
            return new Team();
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

                sTeam.Captain = new SanityReference<Player>() {
                    Ref = team.Captain.Id,
                };

                sTeam.Players = pList;

                var teamSizes = await Sanity.Client.FetchAsync<List<int>>($"*[_type == 'tournament' && count(teams) > 0 && '{team.Id}' in teams[].team->_id].teamSize.min"); 

                foreach(int teamSize in teamSizes.Result)
                {
                    if (teamSize > (team.Players.Length + 1)) {
                        return new SanityResult<Team>{
                            Item = null,
                            Success = false,
                            Reason = "signedUpForTournament"
                        };
                    }
                }
                await Sanity.DocumentSet<SanityTeam>().Update(sTeam).CommitAsync();

                UpdateTeamCache(team);

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
                    if (teamSize > (team.Players.Length + 1)) {
                        return new SanityResult<Team>{
                            Item = null,
                            Success = false,
                            Reason = "signedUpForTournament"
                        };
                    }
                }
                await Sanity.DocumentSet<SanityTeam>().Update(sTeam).CommitAsync();

                UpdateTeamCache(team);

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

                UpdateTeamCache(team, true);

                return true;
            }
            return false;
        }

        public Task<Game[]> GetGamesAsync()
        {
            var lang = System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
            return Cache.GetOrCreateAsync("Sanity_Games_" + lang, async (c) =>
            {
                c.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10);

                var result = await Sanity.DocumentSet<SanityGame>().ToListAsync();

                return result.Select(g => g.ToGame()).ToArray();
            });
        }
    }
}