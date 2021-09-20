using Buk.Gaming.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Buk.Gaming.Repositories
{
    public interface IPlayerRepository
    {
        Task<Player> GetPlayerAsync(string idOrEmail);

        Task<Player> GetPlayerByPersonIdAsync(int personId);

        Task<Player> SavePlayerAsync(Player user);

        Task<Player> UpdateUserAsync(Player user, Player fromUser);

        Task DeletePlayerAsync(string id);

        Task<List<Player>> GetPlayersAsync(IEnumerable<string> ids);
    }
}
