using Buk.Gaming.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace Buk.Gaming.Repositories
{
    public interface ITournamentRepository
    {
        Task<List<TournamentInfo>> GetAllTournamentsAsync();

        Task AddTeamToTournamentAsync(string tournamentId, Participant team);

        Task<Team[]> GetEligibleTeamsAsync(string gameId, string playerId);

        Task AddPlayerToTournamentAsync(string tournamentId, Participant player);

        Task<TournamentAdminInfo> GetAdminInfoAsync(User requester, string tournamentId);
    }
}