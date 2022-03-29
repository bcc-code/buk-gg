using Buk.Gaming.Models;
using System.Threading.Tasks;

namespace Buk.Gaming.Providers
{
    public interface IDiscordProvider
    {
        Task<DiscordUser> SyncUserAsync(Player user);
    }
}
