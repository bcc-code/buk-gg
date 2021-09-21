using Buk.Gaming.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;

namespace Buk.Gaming.Repositories
{
    public interface IOrganizationRepository
    {
        Task<List<Organization>> GetOrganizationsAsync();

        Task SaveOrganizationAsync(Organization organization);

        Task SetImageAsync(Organization organization, Stream image);
    }
}