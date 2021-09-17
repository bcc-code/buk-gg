using Buk.Gaming.Models;
using Buk.Gaming.Providers;
using Buk.Gaming.Repositories;
using Buk.Gaming.Web.Classes;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Buk.Gaming.Web.Services
{
    public class TournamentService
    {
        private readonly ISessionProvider _session;
        private readonly IMemoryCache _cache;

        private readonly ITournamentRepository _tournaments;
        private readonly IOrganizationRepository _organizations;
        private readonly ITeamRepository _teams;

        public TournamentService(ISessionProvider session, IMemoryCache memoryCache, ITournamentRepository tournaments, ITeamRepository teams, IOrganizationRepository organizations)
        {
            _session = session;
            _cache = memoryCache;

            _tournaments = tournaments;
            _teams = teams;
            _organizations = organizations;
        }

        public Task<List<TournamentInfo>> GetTournamentsAsync()
        {
            return _cache.WithSemaphoreAsync("TOURNAMENTS", async () =>
            {
                return await _tournaments.GetAllTournamentsAsync();
            }, TimeSpan.FromMinutes(30));
        }

        public async Task<TournamentInfo> GetTournamentAsync(string id)
        {
            return (await GetTournamentsAsync()).FirstOrDefault(i => i.Id == id);
        }

        public async Task RegisterTeamAsync(string tournamentId, string teamId, string information)
        {
            var tournament = await GetTournamentAsync(tournamentId);

            if (tournament.SignupType != "team")
            {
                throw new Exception("Invalid tournament type");
            }


        }
    }
}
