using Buk.Gaming.Models;
using Buk.Gaming.Repositories;
using Sanity.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buk.Gaming.Sanity
{
    public class SanityPlayerRepository : IPlayerRepository
    {
        public SanityPlayerRepository(SanityDataContext sanity)
        {
            Sanity = sanity;
        }

        public SanityDataContext Sanity { get; }

        public Task DeletePlayerAsync(string id)
        {
            return Sanity.DocumentSet<Player>().DeleteById(id).CommitAsync();
        }

        public Task<Player> GetPlayerAsync(string email)
        {
            return Sanity.DocumentSet<Player>().Where(p => p.Email == email).FirstOrDefaultAsync();
        }

        public Task<Player> GetPlayerByPersonIdAsync(int personId)
        {
            if (personId == 0)
            {
                return Task.FromResult<Player>(null);
            }
            return Sanity.DocumentSet<Player>().Where(p => p.PersonId == personId).FirstOrDefaultAsync();
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
