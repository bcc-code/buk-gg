using Buk.Gaming.Models;
using Buk.Gaming.Providers;
using Buk.Gaming.Repositories;
using Buk.Gaming.Services;
using Buk.Gaming.Web.Classes;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Buk.Gaming.Web.Services
{
    public class TeamService : ITeamService
    {
        private readonly IMemoryCache _cache;

        private readonly ISessionProvider _session;
        private readonly ITeamRepository _teams;
        private readonly IOrganizationRepository _organizations;

        public TeamService(IMemoryCache cache, ISessionProvider session, ITeamRepository teams, IOrganizationRepository organizations)
        {
            _cache = cache;
            _session = session;
            _teams = teams;
            _organizations = organizations;
        }

        private Task<List<Team>> GetAllTeamsAsync()
        {
            return _cache.WithSemaphoreAsync("TEAMS", async () =>
            {
                return await _teams.GetTeamsAsync();
            });
        }



        public async Task<List<Team>> GetTeamsAsync()
        {
            var user = await _session.GetCurrentUser();

            if (user == null)
            {
                throw new Exception("User not valid");
            }
        }

        public async Task<Team> GetTeamAsync(string teamId)
    }
}
