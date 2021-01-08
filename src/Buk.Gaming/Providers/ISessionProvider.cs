using Buk.Gaming.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Buk.Gaming.Providers
{
    public interface ISessionProvider
    {
        Task<User> GetCurrentUser();

        Task<User> GetAuthenticatedUser();
    }
}
