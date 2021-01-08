using Buk.Gaming.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Buk.Gaming.Providers
{
    public interface IDiscordProvider
    {
        Task<DiscordUser> GetUserAsync(string id = null);
        Task<DiscordUser> UpdateUserAsync(User user);

        Task<dynamic> SearchForMembers(string searchString);

        Task<bool> IsConnectedAsync(string id);
    }
}
