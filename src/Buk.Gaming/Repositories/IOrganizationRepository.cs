using Buk.Gaming.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;

namespace Buk.Gaming.Repositories
{
    public interface IOrganizationRepository
    {
        /// <summary>
        /// Get all organizations
        /// </summary>
        /// <returns></returns>
        Task<List<Organization>> GetOrganizationsAsync();

        /// <summary>
        /// Save organization
        /// </summary>
        /// <param name="organization"></param>
        /// <returns></returns>
        Task SaveOrganizationAsync(Organization organization);
    }
}