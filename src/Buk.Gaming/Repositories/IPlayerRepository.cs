using Buk.Gaming.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Buk.Gaming.Repositories
{
    public interface IPlayerRepository
    {
        Task<Player> GetPlayerAsync(string email);

        Task<Player> GetPlayerByPersonIdAsync(int personId);

        Task<Player> SavePlayerAsync(Player user);

        Task DeletePlayerAsync(string id);
    }
}
