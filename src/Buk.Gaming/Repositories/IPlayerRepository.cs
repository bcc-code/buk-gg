using Buk.Gaming.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Buk.Gaming.Repositories
{
    public interface IPlayerRepository
    {
        /// <summary>
        /// Retrieve all players
        /// </summary>
        /// <returns></returns>
        Task<List<Player>> GetPlayersAsync();

        Task<Player> SavePlayerAsync(Player user);

        Task DeletePlayerAsync(string id);
    }
}
