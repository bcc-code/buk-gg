using Buk.Gaming.Classes;
using Buk.Gaming.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Buk.Gaming.Services
{
    public interface ITeamService
    {
        Task<Team> GetTeamAsync(string teamId);

        Task<Dictionary<string, Team>> GetTeamsAsync();

        Task<List<Team>> GetTeamsInOrganizationAsync(string organizationId);

        Task<List<Team>> GetTeamsInGameAsync(string gameId);

        Task<Dictionary<string, Player>> GetPlayersAsync(string teamId);

        Task<Team> CreateTeamAsync(Team.CreateOptions options);

        Task UpdateTeamAsync(string teamId, Team.UpdateOptions options);
    }
}
