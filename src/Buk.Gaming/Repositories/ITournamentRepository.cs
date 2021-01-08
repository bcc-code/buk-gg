using Buk.Gaming.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;


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