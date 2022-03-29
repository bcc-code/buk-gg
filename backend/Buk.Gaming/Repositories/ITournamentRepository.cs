using Buk.Gaming.Models;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Buk.Gaming.Repositories
{
    public interface ITournamentRepository
    {
        Task<List<TournamentInfo>> GetAllTournamentsAsync();

        Task<Participant<Team>> AddTeamToTournamentAsync(string tournamentId, Participant<Team> team);

        Task<Team[]> GetEligibleTeamsAsync(string gameId, string playerId);

        Task<Participant<Player>> AddPlayerToTournamentAsync(string tournamentId, Participant<Player> player);

        Task<TournamentAdminInfo> GetAdminInfoAsync(User requester, string tournamentId);
    }
}