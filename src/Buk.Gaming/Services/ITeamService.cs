using Buk.Gaming.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Buk.Gaming.Services
{
    public interface ITeamService
    {
        /// <summary>
        /// Get team by Id
        /// </summary>
        /// <param name="teamId"></param>
        /// <returns></returns>
        Task<Team> GetTeamAsync(string teamId);

        /// <summary>
        /// Get teams in Organization
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        Task<List<Team>> GetTeamsAsync(string organizationId);

        /// <summary>
        /// Get teams in game
        /// </summary>
        /// <param name="gameId"></param>
        /// <returns></returns>
        Task<List<Team>> GetTeamsInGameAsync(string gameId);

        /// <summary>
        /// Create a new team
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        Task<Team> CreateTeamAsync(Team.CreateOptions options);

        /// <summary>
        /// Add a player to team
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="playerId"></param>
        /// <returns></returns>
        Task AddPlayerAsync(string teamId, string playerId);

        /// <summary>
        /// Remove player from team
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="playerId"></param>
        /// <returns></returns>
        Task RemovePlayerAsync(string teamId, string playerId);

        /// <summary>
        /// Set captain
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="playerId"></param>
        /// <returns></returns>
        Task SetCaptainAsync(string teamId, string playerId);
    }
}
