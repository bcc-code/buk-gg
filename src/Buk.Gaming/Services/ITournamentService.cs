using Buk.Gaming.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Buk.Gaming.Services
{
    public interface ITournamentService
    {
        Task<List<Tournament>> GetTournamentsAsync();

        Task<List<Team>> GetTeamsAsync(string tournamentId);

        Task<List<Participant>> GetParticipantsAsync(string tournamentId);

        Task RegisterAsync(string tournamentId, List<string> information = null);

        Task RegisterTeamAsync(string tournamentId, string teamId, List<string> information = null);
    }
}
