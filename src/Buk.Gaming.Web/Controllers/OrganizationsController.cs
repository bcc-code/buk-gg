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
        public async Task<IActionResult> GetOrganizationsAsync()
        {
            return Ok(await _organizations.GetOrganizationsAsync(true));
        }

        [Route("")]
        [HttpPost]
        public async Task<IActionResult> CreateOrganizationAsync(Organization.CreateOptions options)
        {
            return Ok(await _organizations.CreateOrganizationAsync(options));
        }

        [Route("{organizationId}")]
        [HttpPatch]
        public async Task<IActionResult> UpdateOrganizationAsync(string organizationId, [FromBody] Organization.UpdateOptions options)
        {
            await _organizations.UpdateOrganizationAsync(organizationId, options);
            return Ok();
        }

        [Route("{organizationId}/Join")]
        [HttpGet]
        public async Task<IActionResult> RequestJoinOrganizationAsync(string organizationId)
        {
            await _organizations.AskToJoinAsync(organizationId);
            return Ok();
        }

        [Route("{organizationId}/Members")]
        [HttpPatch]
        public async Task<IActionResult> EditMembersAsync(string organizationId, [FromBody] Organization.MemberOptions options)
        {

        }
    }
}
