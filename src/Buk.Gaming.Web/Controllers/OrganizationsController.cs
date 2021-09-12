using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buk.Gaming.Providers;
using Buk.Gaming.Repositories;
using Buk.Gaming.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Buk.Gaming.Web.Controllers
{
    [Authorize]
    [Route("api/organizations")]
    [ApiController]
    public class OrganizationsController : ControllerBase
    {
        public OrganizationsController(IOrganizationRepository organizationRepository, ISessionProvider session)
        {
            OrganizationRepository = organizationRepository;
            Session = session;
        }

        public IOrganizationRepository OrganizationRepository { get; }
        public ISessionProvider Session { get; }

        [Route("")]
        [HttpPut]
        public async Task<IActionResult> SaveOrganizationAsync(Organization organization)
        {
            var player = await Session.GetCurrentUser();
            if (player == null)
            {
                return Unauthorized();
            }
            return Ok(await OrganizationRepository.SaveOrganizationAsync(player, organization));
        }

        [Route("Mine")]
        [HttpGet]
        public async Task<IActionResult> GetMyOrganizationsAsync()
        {
            var player = await Session.GetCurrentUser();
            if (player == null)
            {
                return Unauthorized();
            }
            List<Organization> organizations = await OrganizationRepository.GetAllOrganizationsAsync();
            return Ok(organizations.Where(o => o.Members.FirstOrDefault(m => m.Player.Id == player.Id) != null));
        }

        [Route("Create")]
        [HttpPut]
        public async Task<IActionResult> CreateOrganizationAsync(Organization organization)
        {
            var user = await Session.GetCurrentUser();
            if (user == null)
            {
                return Unauthorized();
            }
            return Ok(await OrganizationRepository.CreateOrganizationAsync(user, organization));
        }

        [Route("")]
        [HttpGet]
        public async Task<IActionResult> GetOrganizationsAsync()
        {
            if (await Session.GetCurrentUser() == null)
            {
                return Unauthorized();
            }
            return Ok(await OrganizationRepository.GetAllOrganizationsAsync());
        }

        [Route("{organizationId}")]
        [HttpGet]
        public async Task<IActionResult> GetOrganizationAsync(string organizationId)
        {
            if (await Session.GetCurrentUser() == null)
            {
                return Unauthorized();
            }

            List<Organization> organizations = await OrganizationRepository.GetAllOrganizationsAsync();

            return Ok(organizations.FirstOrDefault(o => o.Id == organizationId));
        }
        
        // MEMBERS
        // [Route("{organizationId}/members")]
        // [HttpGet]
        // public async Task<IActionResult> GetMemberAsync(string organizationId)
        // {
        //     if (await Session.GetCurrentUser() == null)
        //     {
        //         return Unauthorized();
        //     }
        //     return Ok(await OrganizationRepository.GetMemberAsync(organizationId));
        // }

        public class AddMember
        {
            public string Id { get; set; }
        }

        [Route("{organizationId}/Members")]
        [HttpPut]
        public async Task<IActionResult> AddMemberAsync(string organizationId, [FromBody] AddMember member)
        {
            User user = await Session.GetCurrentUser();
            if (user == null)
            {
                return Unauthorized();
            }

            var result = await OrganizationRepository.AddPlayerAsync(user, organizationId, member.Id);
            if (result != null)
            {
                return Ok(result);
            } else {
                return Ok(false);
            }
        }


        [Route("{organizationId}/Members")]
        [HttpPatch]
        public async Task<IActionResult> UpdateMemberAsync(string organizationId, Member member) {
            User user = await Session.GetCurrentUser();
            if (user == null)
            {
                return Unauthorized();
            }
            var result = await OrganizationRepository.UpdateMemberAsync(user, organizationId, member);
            if (result != null)
            {
                return Ok(result);
            } else {
                return Ok(false);
            }
        }

        [Route("{organizationId}/Members/{memberId}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteMemberAsync(string organizationId, string memberId)
        {
            User user = await Session.GetCurrentUser();
            if (user == null)
            {
                return Unauthorized();
            }
            return Ok(await OrganizationRepository.RemovePlayerAsync(user, organizationId, memberId));
        }

        [Route("Players/{searchString}")]
        [HttpGet]
        public async Task<IActionResult> SearchForPlayersAsync(string searchString)
        {
            User user = await Session.GetCurrentUser();
            if (user == null)
            {
                return Unauthorized();
            }
            return Ok(await OrganizationRepository.SearchForPlayersAsync(user, searchString));
        }

        public class Base64Image
        {
            public string Image { get; set; }
        }
        
        [Route("{organizationId}/Image")]
        [HttpPut]
        public async Task<IActionResult> UpdateImageAsync(string organizationId, Base64Image dataObject) 
        {
            User user = await Session.GetCurrentUser();
            if (user == null || string.IsNullOrEmpty(dataObject.Image)) {
                return Unauthorized();
            }
            
            byte[] bytes = Convert.FromBase64String(dataObject.Image);

            System.IO.MemoryStream ms = new System.IO.MemoryStream(bytes);

            return Ok(await OrganizationRepository.UpdateImageAsync(user, organizationId, ms));
        }

        // [Route("/invite/{organizationId}/accept")]
        // [HttpGet]
        // public async Task<IActionResult> ManageInvite(string organizationId, string type)
        // {
        //     User user = await Session.GetCurrentUser();
        //     if (user == null)
        //     {
        //         return Unauthorized();
        //     }
        // }

        [Route("{organizationId}/Pending/{type}")]
        [HttpPut]
        public async Task<IActionResult> AddPendingMember(string organizationId, Player player)
        {
            User user = await Session.GetCurrentUser();
            if (user == null)
            {
                return Unauthorized();
            }
            return Ok(await OrganizationRepository.AddPendingPlayerAsync (user, organizationId, player));
        }

        [Route("{organizationId}/Pending/{playerId}")]
        [HttpDelete]
        public async Task<IActionResult> RemovePendingMember(string organizationId, string playerId)
        {
            User user = await Session.GetCurrentUser();
            if (user == null)
            {
                return Unauthorized();
            }
            return Ok(await OrganizationRepository.RemovePendingPlayerAsync (user, organizationId, playerId));
        }

        // [Route("{organizationId}/teams")]
        // [HttpGet]
        // public async Task<IActionResult> GetTeamsAsync(string organizationId)
        // {
        //     if (await Session.GetCurrentUser() == null)
        //     {
        //         return Unauthorized();
        //     }
        //     return Ok(await OrganizationRepository.GetTeamsAsync(organizationId));
        // }

        // [Route("teams/{teamId}")]
        // [HttpGet]
        // public async Task<IActionResult> GetTeamAsync(string teamId)
        // {
        //     if (await Session.GetCurrentUser() == null)
        //     {
        //         return Unauthorized();
        //     }
        //     return Ok(await OrganizationRepository.GetTeamAsync(teamId));
        // }
    }
}
