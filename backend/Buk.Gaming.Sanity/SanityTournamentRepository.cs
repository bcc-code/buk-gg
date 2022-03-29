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
    public class SanityTournamentRepository : ITournamentRepository
    {
        public SanityTournamentRepository(SanityDataContext sanity, IMemoryCache cache)
        {
            Sanity = sanity;
            Cache = cache;
        }

        public SanityDataContext Sanity { get; }
        public IMemoryCache Cache { get; }

        public Task<List<TournamentInfo>> GetAllTournamentsAsync()
        {
            var lang = System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
            return Cache.GetOrCreateAsync("Sanity_Tournament_Info_" + lang, async (c) => {
                
                c.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10);
                var tournaments = await Sanity.DocumentSet<SanityTournament>().Include(t => t.Camp).Where(t => !t.IsDraft() && t.Camp.Value.Active).ToListAsync();
                return tournaments?.Select(t => t.ToTournamentInfo(Sanity.HtmlBuilder)).ToList();

            });
        }

        public async Task<Participant<Team>> AddTeamToTournamentAsync(string tournamentId, Participant<Team> team)
        {
            var teamRef = new TournamentTeam() {
                Team = new SanityReference<SanityTeam>() {
                    Ref = team.Item.Id,
                },
                Information = team.Information.ToList(),
                ToornamentId = team.ToornamentId,
            };

            SanityTournament tournament = await Sanity.DocumentSet<SanityTournament>().GetAsync(tournamentId);

            if (tournament == null || tournament.TeamSize.Min > team.Item.Players.Count + 1 || tournament.TeamSize.Max < team.Item.Players.Count + 1 || tournament.SignupType == "solo" || !tournament.RegistrationOpen) return null;

            if (!string.IsNullOrEmpty(tournament.Id))
            {
                if (tournament.Teams == null) {
                    tournament.Teams = new List<TournamentTeam>();
                }
                if (tournament.Teams.Find(t => t.Team.Ref == team.Item.Id) == null){
                    
                    tournament.Teams.Add(teamRef);

                    await Sanity.DocumentSet<SanityTournament>().Update(tournament).CommitAsync();
                    return team;

                }
            }
            return null;
        }

        public Task<Team[]> GetEligibleTeamsAsync(string tournamentId, string playerId)
        {
            var lang = System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
            return Cache.GetOrCreateAsync("Sanity_Teams_Captain_" + playerId + "_Tournament_" + tournamentId + "_" + lang, async (c) =>
            {
                c.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10);
                string query = $"*[_type == 'team' && '${playerId}' in members[role in ['captain']].player._ref && game._ref in *[_type == 'tournament' && _id == '{tournamentId}'].game._ref]";
                var team = await Sanity.Client.FetchAsync<Team[]>(query);
                return team.Result;
            });
        }

        public async Task<Participant<Player>> AddPlayerToTournamentAsync(string tournamentId, Participant<Player> player)
        {
            var playerRef = new TournamentPlayer() {
                Player = new SanityReference<Player>() {
                    Ref = player.Item.Id,
                    SanityKey = player.Item.Id,
                },
                Information = player.Information.ToList(),
                ToornamentId = player.ToornamentId,
            };

            SanityTournament tournament = await Sanity.DocumentSet<SanityTournament>().GetAsync(tournamentId);

            if (tournament == null || tournament.SignupType == "team" || !tournament.RegistrationOpen) return null;

            if (tournament.SoloPlayers == null)
            {
                tournament.SoloPlayers = new List<TournamentPlayer>();
            }

            if (tournament.SoloPlayers.Find(t => t.Player.Ref == player.Item.Id) != null) return null;
            tournament.SoloPlayers.Add(playerRef);

            await Sanity.DocumentSet<SanityTournament>().Update(tournament).CommitAsync();
            return player;
        }

        // class ParticipantResult<T> {
        //     public Participant<T>[] items { get; set; }

        // }

        public Task<TournamentAdminInfo> GetAdminInfoAsync(User requester, string tournamentId) {
            var lang = System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
            return Cache.GetOrCreateAsync<TournamentAdminInfo>($"Sanity_Tournament_{tournamentId}_Admin_{lang}", async (c) => {
                c.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(20);
                SanityTournament tournament = await Sanity.DocumentSet<SanityTournament>().Where(t => t.Id == tournamentId && t.Responsible.Ref == requester.Id)?.FirstOrDefaultAsync();

                if (tournament != null) {
                    return tournament.ToTournamentAdminInfo(Sanity.HtmlBuilder);
                }

                return new TournamentAdminInfo();
            });
        }

        // public Task<Participant<Team>[]> GetTeamsAsync(User requester, string tournamentId)
        // {
        //     return Cache.GetOrCreateAsync("Sanity_Tournament_" + tournamentId + "_Teams", async c => 
        //     {
        //         c.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10);

        //         string query = $"*[_type == 'tournament' && _id == '{tournamentId}' && responsible._ref == '{requester.Id}']{{teams[]{{information, 'item': team->{{..., organization->, players[]->}}}}}}";

        //         var result = (await Sanity.Client.FetchAsync<ParticipantResult<Team>>(query));

        //         return result.Result.items;
        //     });
        // }
        
        // public Task<Participant<Player>[]> GetPlayersAsync(User requester, string tournamentId)
        // {
        //     return Cache.GetOrCreateAsync("Sanity_Tournament_" + tournamentId + "_Players", async c => 
        //     {
        //         c.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10);

        //         string query = $"*[_type == 'tournament' && _id == '{tournamentId}' && responsible._ref == '{requester.Id}']{{soloPlayers[]{{information, 'item': player->}}}}";

        //         var result = await Sanity.Client.FetchAsync<ParticipantResult<Player>>(query);

        //         return result.Result.items;
        //     });
        // }
    }
}
