﻿using Buk.Gaming.Models;
using System.Threading.Tasks;

namespace Buk.Gaming.Repositories
{
    public interface IPlayerRepository
    {
        Task<Player> GetPlayerAsync(string email);

        Task<Player> GetPlayerByPersonIdAsync(int personId);

        Task<Player> SavePlayerAsync(Player user);

        Task<Player> UpdateUserAsync(Player user, Player fromUser);

        Task DeletePlayerAsync(string id);
    }
}
