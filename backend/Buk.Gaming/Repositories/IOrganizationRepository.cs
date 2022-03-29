using Buk.Gaming.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Buk.Gaming.Repositories
{
    public interface IOrganizationRepository
    {
        Task<List<Organization>> GetAllOrganizationsAsync();

        Task<Organization> SaveOrganizationAsync(User requester, Organization organization);

        Task<Organization> CreateOrganizationAsync(User requester, Organization organization);

        Task<Organization> AddPlayerAsync(User requester, string organizationId, string id);

        Task<Organization> RemovePlayerAsync(User requester, string organizationId, string playerId);

        Task<Organization> UpdateMemberAsync(User requester, string organizationId, Member member);

        Task<Organization> AddPendingPlayerAsync(User requester, string organizationId, Player player);

        Task<Organization> RemovePendingPlayerAsync(User requester, string organizationId, string playerId);

        Task<Organization> UpdateImageAsync(User requester, string organizationId, Stream image);

        Task<List<Player>> SearchForPlayersAsync(User requester, string searchString);
    }
}