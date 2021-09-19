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

        Task<Organization> CreateOrganizationAsync(Organization.CreateOptions options);

        Task DeleteOrganizationAsync(string id);

        Task AddMemberAsync(string organizationId, string playerId);

        Task AskToJoinAsync(string organizationId);

        Task AcceptRequestAsync(string organizationId, string invitationId);
    }
}
