using Buk.Gaming.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace Buk.Gaming.Repositories
{
    public interface ITournamentRepository
    {
        Task<List<Tournament>> GetTournamentsAsync();

        Task AddParticipantAsync(string tournamentId, Participant participant);

        Task RemoveParticipantAsync(string tournamentId, string participantId);
    }
}