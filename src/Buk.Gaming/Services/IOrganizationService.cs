using Buk.Gaming.Classes;
using Buk.Gaming.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Buk.Gaming.Services
{
    public interface IOrganizationService
    {
        Task<Organization> GetOrganizationAsync(string id);

        Task<List<Organization>> GetOrganizationsAsync(bool includePublic = false);

        Task UpdateOrganizationAsync(string id, Organization.UpdateOptions options);

        Task<Organization> CreateOrganizationAsync(Organization.CreateOptions options);

        Task DeleteOrganizationAsync(string id);

        Task EditMembersAsync(string organizationId, MemberList.UpdateOptions options);

        Task AskToJoinAsync(string organizationId);
    }
}
