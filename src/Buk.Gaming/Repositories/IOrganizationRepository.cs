using Buk.Gaming.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;

namespace Buk.Gaming.Repositories
{
    public interface IOrganizationRepository
    {
        Task<List<Organization>> GetAllOrganizationsAsync();

        Task<Organization> SaveOrganizationAsync(User requester, Organization organization);

        Task<Organization> CreateOrganizationAsync(User requester, Organization organization);

        Task<Organization> AddPlayerAsync(User requester, string organizationId, Player player);

        Task<Organization> RemovePlayerAsync(User requester, string organizationId, string playerId);

        Task<Organization> UpdateMemberAsync(User requester, string organizationId, Member member);

        Task<Organization> AddPendingPlayerAsync(User requester, string organizationId, Player player);

        Task<Organization> RemovePendingPlayerAsync(User requester, string organizationId, string playerId);

        Task<Organization> UpdateImageAsync(User requester, string organizationId, Stream image);

        Task<List<Player>> SearchForPlayersAsync(User requester, string searchString);
    }
}