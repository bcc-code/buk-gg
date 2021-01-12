using Buk.Gaming.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Buk.Gaming.Providers
{
    public interface IDiscordProvider
    {
        Task<DiscordUser> SyncUserAsync(Player user);

        Task<List<DiscordMember>> SearchForMembersAsync(string searchString);
    }
}
