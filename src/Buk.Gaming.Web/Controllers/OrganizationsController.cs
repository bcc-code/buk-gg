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
using Buk.Gaming.Services;
using Buk.Gaming.Models.Views;
using Buk.Gaming.Extensions;

namespace Buk.Gaming.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationsController : BaseControllerBase
    {
        private readonly IOrganizationService _organizations;

        public OrganizationsController(ISessionProvider session, IOrganizationService organizations) : base(session)
        {
            _organizations = organizations;
        }

        [Route("")]
        [HttpGet]
        [ProducesDefaultResponseType(typeof(IEnumerable<OrganizationView>))]
        public async Task<IActionResult> GetOrganizationsAsync()
        {
            return Ok((await _organizations.GetOrganizationsAsync(true)).Select(i => i.View()));
        }

        [Route("")]
        [HttpPost]
        [ProducesDefaultResponseType(typeof(OrganizationView))]
        public async Task<IActionResult> CreateOrganizationAsync(Organization.CreateOptions options)
        {
            return Ok((await _organizations.CreateOrganizationAsync(options)).View());
        }

        [Route("{organizationId}")]
        [HttpPatch]
        public async Task<IActionResult> UpdateOrganizationAsync(string organizationId, [FromBody] Organization.UpdateOptions options)
        {
            await _organizations.UpdateOrganizationAsync(organizationId, options);

            if (options.Members != null)
            {
                await _organizations.EditMembersAsync(organizationId, options.Members);
            }

            return Ok();
        }

        [Route("{organizationId}/Join")]
        [HttpGet]
        public async Task<IActionResult> RequestJoinOrganizationAsync(string organizationId)
        {
            await _organizations.AskToJoinAsync(organizationId);
            return Ok();
        }

        //[Route("{organizationId}/Members")]
        //[HttpPatch]
        //public async Task<IActionResult> EditMembersAsync(string organizationId, [FromBody] MemberList.MemberOptions options)
        //{
        //    await _organizations.EditMembersAsync(organizationId, options);
        //    return Ok();
        //}
    }
}
