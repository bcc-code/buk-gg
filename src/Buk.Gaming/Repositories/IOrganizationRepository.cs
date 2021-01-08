using Buk.Gaming.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace Buk.Gaming.Repositories
{
    public interface IOrganizationRepository
    {
        // ORGANIZATIONS
        Task<Organization[]> GetOrganizationsAsync();

		//Task<Organization> GetOrganizationAsync(string organizationId);
				
        Task<Organization> SaveOrganizationAsync(Organization organization, Player player);

        Task<Organization> CreateOrganizationAsync(Organization organization, Player player);

		Task<Organization[]> GetPlayerOrganizationsAsync(Player player, string role = "");

        Task<string> UpdateImageAsync(User requester, string organizationId, System.IO.Stream image);

        // MEMBERS
        Task<Member> GetMemberAsync(Player player);

        Task<Member> AddMemberAsync(User requester, string organizationId, Player player);

        Task<Member> UpdateMemberAsync(User requester, string organizationId, Member member);

        Task<bool> DeleteMemberAsync(User requester, string organizationId, string memberId);

        Task<Player[]> SearchForPlayersAsync(User requester, string searchString);

        Task<bool> AddPendingMember(User requester, string organizationId, Player player, string type);

        Task<bool> RemovePendingMember(User requester, string organizationId, string playerId);
    }
}