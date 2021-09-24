using Buk.Gaming.Models;
using Buk.Gaming.Providers;
using Buk.Gaming.Repositories;
using Buk.Gaming.Services;
using Buk.Gaming.Web.Classes;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Buk.Gaming.Web.Services
{
    public class PlayerService : BaseService, IPlayerService
    {
        private readonly IPlayerRepository _players;

        public PlayerService(IMemoryCache cache, ISessionProvider session, IPlayerRepository players) : base(cache, session)
        {
            _players = players;
        }

        public async Task DeletePlayerAsync(string id)
        {
            await GetPlayerAsync(id);
            await _players.DeletePlayerAsync(id);

            var players = await GetPlayersAsync();

            players.Remove(id);
        }

        public async Task<Player> GetPlayerAsync(string id)
        {
            return (await GetPlayersAsync()).GetValueOrDefault(id) ?? throw new Exception("Player not found");
        }

        public async Task<Player> GetPlayerByPersonIdAsync(int personId)
        {
            return (await GetPlayersAsync()).Values.FirstOrDefault(i => i.PersonId == personId);
        }

        public async Task<Player> GetPlayerByEmailAsync(string email)
        {
            return (await GetPlayersAsync()).Values.FirstOrDefault(i => i.Email == email);
        }

        public Task<Dictionary<string, Player>> GetPlayersAsync()
        {
            return Cache.WithSemaphoreAsync("PLAYERS", async () =>
            {
                return (await _players.GetPlayersAsync()).ToDictionary(p => p.Id, p => p);
            }, TimeSpan.FromMinutes(60));
        }

        public async Task UpdatePlayerAsync(Player player, Player.UpdateOptions options)
        {
            player.Nickname = options.Nickname ?? player.Nickname;
            player.PhoneNumber = options.PhoneNumber ?? player.PhoneNumber;
            player.DiscordId = options.DiscordId ?? player.DiscordId;
            player.DiscordUser = options.DiscordUser ?? player.DiscordUser;

            await SavePlayerAsync(player);
        }

        public async Task SavePlayerAsync(Player player)
        {
            await _players.SavePlayerAsync(player);

            (await GetPlayersAsync())[player.Id] ??= player;
        }
    }
}
