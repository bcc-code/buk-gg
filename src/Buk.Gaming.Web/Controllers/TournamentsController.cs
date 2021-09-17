using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buk.Gaming.Providers;
using Buk.Gaming.Repositories;
using Buk.Gaming.Models;
using Buk.Gaming.Toornament;
using Buk.Gaming.Toornament.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Buk.Gaming.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TournamentsController : ControllerBase
    {
        public TournamentsController(ToornamentClient toornament, ITournamentRepository tournamentInfo, ISessionProvider session, ITeamRepository teams)
        {
            Toornament = toornament;
            TournamentInfo = tournamentInfo;
            Session = session;
            _teams = teams;
        }

        public ToornamentClient Toornament { get; }
        public ITournamentRepository TournamentInfo { get; }
        public ISessionProvider Session { get; }
        private readonly ITeamRepository _teams;
        private readonly IOrganizationRepository _organizations;

        [Route("")]
        [HttpGet]
        public async Task<IActionResult> GetTournamentsAsync()
        {
            if (await Session.GetCurrentUser() == null)
            {
                return Unauthorized();
            }
            return Ok(await TournamentInfo.GetAllTournamentsAsync());
            // return Ok(await Toornament.Organizer.GetTournamentsAsync());
        }

        [Route("{tournamentId}")]
        [HttpGet]
        public async Task<IActionResult> GetTournamentsAsync(string tournamentId)
        {
            if (await Session.GetCurrentUser() == null)
            {
                return Unauthorized();
            }
            var tournaments = await TournamentInfo.GetAllTournamentsAsync();
            var tournament = tournaments.FirstOrDefault(t => t.Id == tournamentId || t.Slug == tournamentId || t.ToornamentId == tournamentId);
            return Ok(tournament);
        }

        public class RegisterTeamRequest
        {
            public string TeamId { get; set; }
        }

        [Route("{tournamentId}/teams")]
        [HttpPut]
        public async Task<IActionResult> AddTeamToTournamentAsync(string tournamentId, [FromBody] RegisterTeamRequest request)
        {
            var user = await Session.GetCurrentUser();
            if (user == null)
            {
                return Unauthorized();
            }
            var team = (await _teams.GetTeamsAsync()).FirstOrDefault(i => i.Id == request.TeamId);

            if (team.CaptainId != user.Id)
            {
                var organization = (await _organizations.GetAllOrganizationsAsync()).FirstOrDefault(o => o.Id == team.OrganizationId);
                var roles = new string[] { "owner", "officer" };
                
                if (organization.Members.FirstOrDefault(i => i.Player.Id == user.Id && roles.Contains(i.Role)) == null) {
                    throw new Exception("No access");
                }
            }

            var tournament = (await TournamentInfo.GetAllTournamentsAsync()).FirstOrDefault(t => t.Id == tournamentId || t.Slug == tournamentId || t.ToornamentId == tournamentId);
            Toornament.Participant participant = new Toornament.Participant{Identifier = request.TeamId};
            if (!string.IsNullOrEmpty(tournament?.ToornamentId)) 
            {
                team = await Toornament.Organizer.AddParticipantAsync(tournament.ToornamentId, team);
            }
            var participant = new Participant<Team>{Item = addTeam.Item, Information = addTeam.Information, ToornamentId = team.Id};
            return Ok(await TournamentInfo.AddTeamToTournamentAsync(tournamentId, participant));
        }

        public class RegisterPlayerRequest
        {
            public string PlayerId { get; set; }
        }

        [Route("{tournamentId}/players")]
        [HttpPut]
        public async Task<IActionResult> AddPlayerToTournamentAsync(string tournamentId, [FromBody] RegisterPlayerRequest request)
        {
            var user = await Session.GetCurrentUser();
            if (user == null)
            {
                return Unauthorized();
            }
            var tournament = (await TournamentInfo.GetAllTournamentsAsync()).FirstOrDefault(t => t.Id == tournamentId || t.Slug == tournamentId || t.ToornamentId == tournamentId);
            Toornament.Participant player = new Toornament.Participant{Identifier = addPlayer.Item.Id, Name = addPlayer.Item.Name};
            if (!string.IsNullOrEmpty(tournament?.ToornamentId)) 
            {
                player = await Toornament.Organizer.AddParticipantAsync(tournament.ToornamentId, player);
            }
            var participant = new Participant<Player>{Item = addPlayer.Item, Information = addPlayer.Information, ToornamentId = player.Id};
            return Ok(await TournamentInfo.AddPlayerToTournamentAsync(tournamentId, participant));
        }

        [HttpGet]
        [Route("{tournamentId}/stages")]
        public async Task<IActionResult> GetStagesAsync(string tournamentId)
        {
            if (await Session.GetCurrentUser() == null)
            {
                return Unauthorized();
            }
            var stages = await Toornament.Organizer.GetStagesAsync(tournamentId);
            return Ok((stages ?? new List<Stage>()).OrderBy(s => s.Number));
        }

        [HttpGet]
        [Route("{tournamentId}/participants")]
        public async Task<IActionResult> GetParticipantsAsync(string tournamentId)
        {
            if (await Session.GetCurrentUser() == null)
            {
                return Unauthorized();
            }
            var participants = await Toornament.Organizer.GetParticipantsAsync(tournamentId);
            return Ok((participants ?? new List<Participant>()).OrderBy(s => s.Id));
        }

        [Route("{tournamentId}/captain")]
        [HttpGet]
        public async Task<IActionResult> GetEligbleTeamsAsync(string tournamentId)
        {
            User player = await Session.GetCurrentUser();
            if (player == null)
            {
                return Unauthorized();
            }
            if (tournamentId == "null")
            {
                return Ok(false);
            }
            return Ok(await TournamentInfo.GetEligibleTeamsAsync(tournamentId, player.Id));
        }

        [Route("{tournamentId}/admin")]
        [HttpGet]
        public async Task<IActionResult> GetAdminInfoAsync(string tournamentId)
        {
            User player = await Session.GetCurrentUser();
            if (player == null)
            {
                return Unauthorized();
            }
            return Ok(await TournamentInfo.GetAdminInfoAsync(player, tournamentId));
        }
    }
}