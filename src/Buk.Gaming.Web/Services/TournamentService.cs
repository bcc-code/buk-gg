using Buk.Gaming.Extensions;
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
    public class TournamentService : BaseService, ITournamentService
    {
        private readonly ITournamentRepository _tournaments;
        private readonly IPlayerRepository _players;
        private readonly IOrganizationService _organizations;
        private readonly ITeamService _teams;

        public TournamentService(ISessionProvider session, IMemoryCache memoryCache, ITournamentRepository tournaments, ITeamService teams, IPlayerRepository players, IOrganizationService organizations) : base(memoryCache, session)
        {
            _tournaments = tournaments;
            _teams = teams;
            _players = players;
            _organizations = organizations;
        }

        public async Task<(List<Participant> participants, List<Player> players)> GetParticipantsAsync(string tournamentId)
        {
            var user = await Session.GetCurrentUser();

            var tournament = await GetTournamentAsync(tournamentId);

            if (user.Id != tournament.ResponsibleId)
            {
                throw new UnauthorizedAccessException("No access, stay away!");
            }



            return new(tournament.Participants, tournament.SignupType == "player" ? await _players.GetPlayersAsync(tournament.Participants.Select(i => i.Id)) : null);
        }

        public async Task<Tournament> GetTournamentAsync(string tournamentId)
        {
            return (await GetTournamentsAsync()).FirstOrDefault(t => t.Id == tournamentId);
        }

        public Task<List<Team>> GetTeamsAsync(string tournamentId)
        {
            return Cache.WithSemaphoreAsync("TOURNAMENT_TEAMS_" + tournamentId, async () =>
            {
                return await _teams.GetTeamsInTournamentAsync(tournamentId);
            }, TimeSpan.FromMinutes(30));
        }

        public Task<List<Tournament>> GetTournamentsAsync()
        {
            throw new NotImplementedException();
        }
        
        public Task RegisterAsync(string tournamentId, string information = null)
        {
            throw new NotImplementedException();
        }

        public Task RegisterTeamAsync(string tournamentId, string teamId, string information = null)
        {
            throw new NotImplementedException();
        }
    }
}
