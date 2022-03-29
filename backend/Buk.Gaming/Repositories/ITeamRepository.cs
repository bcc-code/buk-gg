using Buk.Gaming.Models;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Buk.Gaming.Repositories
{
    public interface ITeamRepository
    {
        // TEAMS
        Task<Team> GetTeamAsync(string teamId);

        Task<List<Team>> GetTeamsAsync();

        Task<List<Team>> GetTeamsInGameAsync(string gameId);

        Task<Team> AddTeamAsync(User requester, Team team);

        Task<Team> UpdateTeamAsync(User requester, Team team);

        Task<bool> DeleteTeamAsync(User requester, string teamId);

        Task<List<Game>> GetGamesAsync();

        // // MEMBERS
        // Task<Member> GetMemberAsync(Player player);

        // Task<Member> AddMemberAsync(User requester, string teamId, Player player);

        // Task<Member> UpdateMemberAsync(User requester, string teamId, Player player);

        // Task<bool> RemoveMemberAsync(User requester, string teamId, Player player);
    }
}