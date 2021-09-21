using Buk.Gaming.Models;
using Buk.Gaming.Repositories;
using Buk.Gaming.Sanity.Extensions;
using Buk.Gaming.Sanity.Models;
using Microsoft.Extensions.Caching.Memory;
using Sanity.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buk.Gaming.Sanity
{
    public class SanityPlayerRepository : SanityRepository, IPlayerRepository
    {
        public SanityPlayerRepository(SanityDataContext sanity, IMemoryCache memoryCache) : base(sanity, memoryCache)
        {

        }

        public Task DeletePlayerAsync(string id)
        {
            return Sanity.DocumentSet<Player>().DeleteById(id).CommitAsync();
        }

        public Task<Player> GetPlayerAsync(string idOrEmail)
        {
            if (Guid.TryParse(idOrEmail, out Guid id))
            {
                return Sanity.DocumentSet<Player>().GetAsync(id.ToString());
            }
            return Sanity.DocumentSet<Player>().Where(p => p.Email == idOrEmail).FirstOrDefaultAsync();
        }

        public Task<Player> GetPlayerByPersonIdAsync(int personId)
        {
            if (personId == 0)
            {
                return Task.FromResult<Player>(null);
            }
            return Sanity.DocumentSet<Player>().Where(p => p.PersonId == personId).FirstOrDefaultAsync();
        }

        public async Task<List<Player>> GetPlayersAsync()
        {
            var lastActive = DateTime.UtcNow.AddYears(-1);

            return (await Sanity.Client.FetchAsync<List<SanityPlayer>>("*[_type == 'player' && dateLastActive < $lastActive]", new { lastActive })).Result.Select(p => p.ToPlayer()).ToList();
        }

        public async Task<Player> SavePlayerAsync(Player user)
        {
            if (string.IsNullOrEmpty(user.Id))
            {
                user.Id = Guid.NewGuid().ToString();
                await Sanity.DocumentSet<Player>().Create(user).CommitAsync();
                return user;
            }

            await Sanity.DocumentSet<Player>().PatchById(user.Id, p => p.Set = user).CommitAsync();
            return user;
        }

        public async Task<Player> UpdateUserAsync(Player user, Player fromUser)
        {
            user.Nickname = fromUser.Nickname;
            user.PhoneNumber = fromUser.PhoneNumber;
            user.MoreDiscordUsers = fromUser.MoreDiscordUsers;
            user.DiscordId = fromUser.DiscordId;
            user.DiscordUser = fromUser.DiscordUser;
            
            await Sanity.DocumentSet<Player>().PatchById(user.Id, p => p.Set = user).CommitAsync();
            return user;
        }
    }
}
