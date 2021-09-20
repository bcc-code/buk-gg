using Buk.Gaming.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace Buk.Gaming.Repositories
{
    public interface ITeamRepository
    {
        Task<List<Team>> GetTeamsForOrganizationAsync(string organizationId);

        Task<List<Team>> GetTeamsForOrganizationsAsync(IEnumerable<string> organizationIds);

        Task<List<Team>> GetTeamsForGameAsync(string gameId);

        Task<Team> GetTeamAsync(string teamId);

        Task SaveOrCreateTeamAsync(Team team);
    }
}