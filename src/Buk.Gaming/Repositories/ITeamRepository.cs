using Buk.Gaming.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace Buk.Gaming.Repositories
{
    public interface ITeamRepository
    {
        Task<List<Team>> GetTeamsAsync();

        Task SaveTeamAsync(Team team);

        Task DeleteTeamAsync(string teamId);
    }
}