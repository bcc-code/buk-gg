using Buk.Gaming.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace Buk.Gaming.Repositories
{
    public interface IOrganizationRepository
    {
        // ORGANIZATIONS
        Task<List<Organization>> GetAllOrganizationsAsync();

		Task<List<Organization>> GetPlayerOrganizationsAsync(Player player, string role = "");
				
        Task<Organization> SaveOrganizationAsync(Organization organization, Player player);

        Task<Organization> CreateOrganizationAsync(Organization organization, Player player);


        Task<Organization> UpdateImageAsync(User requester, string organizationId, System.IO.Stream image);

        // MEMBERS
        Task<Organization> AddMemberAsync(User requester, string organizationId, Player player);

        Task<Organization> UpdateMemberAsync(User requester, string organizationId, Member member);

        Task<Organization> DeleteMemberAsync(User requester, string organizationId, string memberId);

        Task<List<Player>> SearchForPlayersAsync(User requester, string searchString);

        Task<Organization> AddPendingMember(User requester, string organizationId, Player player, string type);

        Task<Organization> RemovePendingMember(User requester, string organizationId, string playerId);
    }
}