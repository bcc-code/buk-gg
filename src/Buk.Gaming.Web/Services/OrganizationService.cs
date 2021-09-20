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
    public class OrganizationService : BaseService, IOrganizationService
    {
        private readonly IOrganizationRepository _organizations;

        public OrganizationService(IMemoryCache cache, ISessionProvider session, IOrganizationRepository organizations): base(cache, session)
        {
            _organizations = organizations;
        }

        private Task<Dictionary<string, Organization>> GetAllOrganizationsAsync()
        {
            return Cache.WithSemaphoreAsync("ORGANIZATIONS", async () =>
            {
                var orgs = await _organizations.GetOrganizationsAsync();

                return orgs.ToDictionary(o => o.Id, o => o);
            }, TimeSpan.FromMinutes(30));
        }

        public Task AcceptRequestAsync(string organizationId, string invitationId)
        {
            throw new NotImplementedException();
        }

        public Task AddMemberAsync(string organizationId, string playerId)
        {
            throw new NotImplementedException();
        }

        public Task AskToJoinAsync(string organizationId)
        {
            throw new NotImplementedException();
        }

        public Task<Organization> CreateOrganizationAsync(Organization.CreateOptions options)
        {
            throw new NotImplementedException();
        }

        public Task DeleteOrganizationAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<Organization> GetOrganizationAsync(string id)
        {
            throw new NotImplementedException();
        }
    }
}
