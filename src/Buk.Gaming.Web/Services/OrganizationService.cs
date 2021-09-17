using Buk.Gaming.Models;
using Buk.Gaming.Providers;
using Buk.Gaming.Repositories;
using Buk.Gaming.Web.Classes;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Buk.Gaming.Web.Services
{
    public class OrganizationService
    {
        private readonly IMemoryCache _cache;
        private readonly ISessionProvider _session;
        private readonly IOrganizationRepository _organizations;

        public OrganizationService(IMemoryCache cache, ISessionProvider session, IOrganizationRepository organizations)
        {
            _cache = cache;
            _session = session;
            _organizations = organizations;
        }

        private Task<List<Organization>> GetAllOrganizations()
        {
            return _cache.WithSemaphoreAsync("ORGANIZATIONS", async () =>
            {
                return await _organizations.GetAllOrganizationsAsync();
            });
        }

        public async Task<List<Organization>> GetOrganizationsAsync()
        {
            var user = await _session.GetCurrentUser();

            if (user == null)
            {
                throw new Exception("User not found");
            }

            return (await GetAllOrganizations()).Where(i => i.IsPublic || i.Members.Any(m => m.Player.Id == user.Id)).ToList();
        }
    }
}
