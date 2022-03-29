using Buk.Gaming.Models;
using System.Threading.Tasks;

namespace Buk.Gaming.Providers
{
    public interface ISessionProvider
    {
        Task<User> GetCurrentUser();

        Task<User> GetAuthenticatedUser();
    }
}
