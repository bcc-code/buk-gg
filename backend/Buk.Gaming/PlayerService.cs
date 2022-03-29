using Buk.Gaming.Models;
using Buk.Gaming.Providers;
using Buk.Gaming.Repositories;
using System;
using System.Threading.Tasks;

namespace Buk.Gaming
{
    public class PlayerService
    {
        public PlayerService(IPlayerRepository players, ISessionProvider session)
        {
            Players = players;
            Session = session;
        }

        public IPlayerRepository Players { get; }
        public ISessionProvider Session { get; }

        public async Task<Player> SaveUserAsync(Player player)
        {
            var currentUser = await Session.GetCurrentUser();
            if (currentUser?.Email == player.Email)
            {
                if (!player.IsRegistered)
                {
                    player.DateRegistered = DateTimeOffset.Now;
                }
                return await Players.SavePlayerAsync(player);
            }
            throw new Exception("User is not authorized.");
        }

        public async Task<Player> UpdateCurrentUserAsync(Player player)
        {
            var currentUser = await Session.GetCurrentUser();
            return await Players.UpdateUserAsync(currentUser, player);
        }
    }
}
