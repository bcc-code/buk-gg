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
    public class TeamService : BaseService, ITeamService
    {
        private readonly ITeamRepository _teams;
        private readonly IOrganizationService _organizations;

        public TeamService(IMemoryCache cache, ISessionProvider session, ITeamRepository teams, IOrganizationService organizations): base(cache, session)
        {
            _teams = teams;
            _organizations = organizations;
        }

        public Task AddPlayerAsync(string teamId, string playerId)
        {
            var user = await Session.GetCurrentUser();

            
        }

        public Task<Team> CreateTeamAsync(Team.CreateOptions options)
        {
            throw new NotImplementedException();
        }

        public Task<Team> GetTeamAsync(string teamId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Team>> GetTeamsAsync(string organizationId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Team>> GetTeamsInGameAsync(string gameId)
        {
            throw new NotImplementedException();
        }

        public Task RemovePlayerAsync(string teamId, string playerId)
        {
            throw new NotImplementedException();
        }

        public Task SetCaptainAsync(string teamId, string playerId)
        {
            throw new NotImplementedException();
        }
    }
}
