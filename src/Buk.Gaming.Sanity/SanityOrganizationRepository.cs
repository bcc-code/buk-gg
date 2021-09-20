using Buk.Gaming.Models;
using Buk.Gaming.Repositories;
using Buk.Gaming.Sanity.Models;
using Microsoft.Extensions.Caching.Memory;
using Sanity.Linq;
using Sanity.Linq.Extensions;
using Sanity.Linq.CommonTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Buk.Gaming.Sanity.Extensions;

namespace Buk.Gaming.Sanity
{
    public class SanityOrganizationRepository : SanityRepository, IOrganizationRepository
    {
        public SanityOrganizationRepository(SanityDataContext sanity, IMemoryCache cache): base(sanity, cache)
        {

        }

        public async Task<List<Organization>> GetOrganizationsAsync()
        {
            return (await Sanity.DocumentSet<SanityOrganization>().Where(i => !i.Id.StartsWith("draft")).ToListAsync()).Select(i => i.ToOrganization()).ToList();
        }

        public async Task<Organization> GetOrganizationAsync(string organizationId)
        {
            return (await Sanity.DocumentSet<SanityOrganization>().GetAsync(organizationId)).ToOrganization();
        }

        public async Task SaveOrganizationAsync(Organization organization)
        {
            var org = organization.ToSanity();
            if (string.IsNullOrEmpty(org.Id))
            {
                org.Id = Guid.NewGuid().ToString();
                await Sanity.DocumentSet<SanityOrganization>().Create(org).CommitAsync();
            } else
            {
                await Sanity.DocumentSet<SanityOrganization>().Update(org).CommitAsync();
            }
        }
    }
}
