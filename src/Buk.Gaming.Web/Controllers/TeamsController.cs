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
using Buk.Gaming.Extensions;
using Buk.Gaming.Models.Views;

namespace Buk.Gaming.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TeamsController : BaseControllerBase
    {
        private readonly ITeamService _teams;
        private readonly IOrganizationService _organizations;

        public TeamsController(ISessionProvider session, ITeamService teams, IOrganizationService organizations): base(session)
        {
            _teams = teams;
            _organizations = organizations;
        }

        [Route("")]
        [HttpGet]
        public async Task<IActionResult> GetTeamsAsync()
        {
            await Session.GetCurrentUser();
            List<Team> teams = new();

            var orgs = await _organizations.GetOrganizationsAsync();

            foreach (var org in orgs)
            {
                teams.AddRange(await _teams.GetTeamsInOrganizationAsync(org.Id));
            }

            return Ok(teams.Select(i => i.View()));
        }

        [Route("Game/{gameId}")]
        [HttpGet]
        public async Task<IActionResult> GetTeamsInGameAsync(string gameId)
        {
            await Session.GetCurrentUser();
            List<TeamView> teams = new();

            foreach (var t in await _teams.GetTeamsInGameAsync(gameId))
            {
                teams.Add(t.View(await _teams.GetPlayersAsync(t.Id)));
            }

            return Ok(teams);
        }

        [Route("Organization/{organizationId}")]
        [HttpGet]
        public async Task<IActionResult> GetTeamsInOrganizationAsync(string organizationId)
        {
            await Session.GetCurrentUser();
            List<TeamView> teams = new();

            foreach (var t in await _teams.GetTeamsInOrganizationAsync(organizationId))
            {
                teams.Add(t.View(await _teams.GetPlayersAsync(t.Id)));
            }

            return Ok(teams);
        }

        [Route("")]
        [HttpPost]
        public async Task<IActionResult> CreateTeamAsync([FromBody] Team.CreateOptions options)
        {
            return Ok(await _teams.CreateTeamAsync(options));
        }

        [Route("{teamId}")]
        [HttpDelete]
        public Task<IActionResult> DeleteTeamAsync(string teamId)
        {
            throw new NotImplementedException();
        }

        [Route("{teamId}/Players")]
        [HttpPost]
        public async Task<IActionResult> AddPlayersAsync(string teamId, [FromBody] List<string> playerIds)
        {
            await _teams.AddPlayersAsync(teamId, playerIds);
            return Ok();
        }

        [Route("{teamId}/Players")]
        [HttpPatch]
        public async Task<IActionResult> RemovePlayersAsync(string teamId, [FromBody] List<string> playerIds)
        {
            await _teams.RemovePlayersAsync(teamId, playerIds);
            return Ok();
        }

        [Route("{teamId}")]
        [HttpPatch]
        public async Task<IActionResult> UpdateTeamAsync(string teamId, [FromBody] Team.UpdateOptions options)
        {
            await _teams.UpdateTeamAsync(teamId, options);
            return Ok();
        } 
    }
}
