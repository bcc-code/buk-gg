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
    [Route("api/teams")]
    [ApiController]
    public class TeamsController : ControllerBase
    {
        public TeamsController(ITeamRepository teamRepository, ISessionProvider session)
        {
            TeamRepository = teamRepository;
            Session = session;
        }

        public ITeamRepository TeamRepository { get; }
        public ISessionProvider Session { get; }

        [Route("")]
        [HttpGet]
        public async Task<IActionResult> GetTeams()
        {
            var user = await Session.GetCurrentUser();
            if (user == null)
            {
                return Unauthorized();
            }

            var teams = await TeamRepository.GetTeamsAsync();

            var myTeams = teams.Where(t => t.Organization.IsPublic == true).ToArray();

            return Ok(myTeams);
        }

        [Route("{organizationId}")]
        [HttpGet]
        public async Task<IActionResult> GetTeamsInOrganizationAsync(string organizationId)
        {
            var user = await Session.GetCurrentUser();
            if (user == null)
            {
                return Unauthorized();
            }

            var teams = await TeamRepository.GetTeamsAsync();

            var orgTeams = teams?.Where(t => t.Organization.Id == organizationId).ToArray();

            return Ok(orgTeams ?? default);
        }

        [Route("game/{gameId}")]
        [HttpGet]
        public async Task<IActionResult> GetTeamsInGameAsync(string gameId)
        {
            var user = await Session.GetCurrentUser();
            if (user == null)
            {
                return Unauthorized();
            }
            return Ok(await TeamRepository.GetTeamsInGameAsync(gameId));
        }

        [Route("add")]
        [HttpPut]
        public async Task<IActionResult> AddTeamAsync(Team team)
        {
            var user = await Session.GetCurrentUser();
            if (user == null)
            {
                return Unauthorized();
            }
            return Ok(await TeamRepository.AddTeamAsync(user, team));
        }

        [Route("update")]
        [HttpPut]
        public async Task<IActionResult> UpdateTeamAsync(Team team)
        {
            var user = await Session.GetCurrentUser();
            if (user == null)
            {
                return Unauthorized();
            }

            return Ok(await TeamRepository.UpdateTeamAsync(user, team));
        }

        [Route("delete")]
        [HttpPost]
        public async Task<IActionResult> DeleteTeamAsync(Team team)
        {
            var user = await Session.GetCurrentUser();
            if (user == null)
            {
                return Unauthorized();
            }
            return Ok(await TeamRepository.DeleteTeamAsync(user, team));
        }

        [Route("games")]
        [HttpGet]
        public async Task<IActionResult> GetGamesAsync()
        {
            var user = await Session.GetCurrentUser();
            if (user == null)
            {
                return Unauthorized();
            }
            return Ok(await TeamRepository.GetGamesAsync());
        }

    }
}
