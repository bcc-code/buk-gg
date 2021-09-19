using Buk.Gaming.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace Buk.Gaming.Repositories
{
    public interface ITournamentRepository
    {
        Task<List<Tournament>> GetAllTournamentsAsync();

        Task<Tournament> GetTournamentAsync(string id);

        Task SaveOrCreateTournamentAsync(Tournament tournament);
    }
}