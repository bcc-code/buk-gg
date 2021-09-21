using Buk.Gaming.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buk.Gaming.Services
{
    public interface IPlayerService
    {
        Task<Player> GetPlayerByPersonIdAsync(int personId);

        Task<Player> GetPlayerByEmailAsync(string email);

        Task<Player> GetPlayerAsync(string id);

        Task<Dictionary<string, Player>> GetPlayersAsync();

        Task UpdatePlayerAsync(Player player, Player.UpdateOptions options);

        Task SavePlayerAsync(Player player);

        Task DeletePlayerAsync(string id);
    }
}
