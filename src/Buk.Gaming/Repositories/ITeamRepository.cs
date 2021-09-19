using Buk.Gaming.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace Buk.Gaming.Repositories
{
    public interface ITeamRepository
    {
        // TEAMS
        Task<Team> GetTeamAsync(string teamId);

        Task<List<Team>> GetTeamsAsync();

        Task SaveOrCreateTeamAsync(Team team);
    }
}