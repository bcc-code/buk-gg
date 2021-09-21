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
using Buk.Gaming.Services;
using Buk.Gaming.Extensions;
using Buk.Gaming.Models.Views;

namespace Buk.Gaming.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TournamentsController : BaseControllerBase
    {
        private readonly ToornamentClient _toornamentClient;
        private readonly ITournamentService _tournaments;
        private readonly IPlayerService _players;
        private readonly ITeamService _teams;

        public TournamentsController(ISessionProvider session, ToornamentClient toornament, ITournamentService tournaments, IPlayerService players, ITeamService teams): base(session)
        {
            _toornamentClient = toornament;
            _tournaments = tournaments;
            _players = players;
            _teams = teams;
        }

        [Route("")]
        [HttpGet]
        public async Task<IActionResult> GetTournamentsAsync()
        {
            await Session.GetCurrentUser();

            List<TournamentView> tournaments = new();
            var ts = await _tournaments.GetTournamentsAsync();

            foreach (var t in ts)
            {
                tournaments.Add(t.View(t.SignupType.Value == SignupType.Team.Value ? await _tournaments.GetTeamsAsync(t.Id) : null));
            }

            return Ok(tournaments);
        }

        [Route("{tournamentId}/Participants")]
        [HttpGet]
        public async Task<IActionResult> GetTournamenAsync(string tournamentId)
        {
            await Session.GetCurrentUser();

            List<ParticipantView> participants = new();

            foreach (var participant in await _tournaments.GetParticipantsAsync(tournamentId))
            {
                if (participant.Type.Equals(ParticipantType.Player))
                {
                    participants.Add(participant.View(await _players.GetPlayerAsync(participant.Id)));
                } else if (participant.Type.Equals(ParticipantType.Team))
                {
                    participants.Add(
                        participant.View(await _teams.GetTeamAsync(participant.Id), 
                        await _teams.GetPlayersAsync(participant.Id))
                    );
                }
            }

            return Ok(participants);
        }

        [Route("{tournamentId/Participants")]
        [HttpPost]
        public async Task<IActionResult> RegisterAsync(string tournamentId, [FromBody] List<string> information)
        {
            await _tournaments.RegisterAsync(tournamentId, information);
            return Ok();
        }

        [Route("{tournamentId}/Participants/{teamId}")]
        [HttpPost]
        public async Task<IActionResult> RegisterAsync(string tournamentId, string teamId, [FromBody] List<string> information)
        {
            await _tournaments.RegisterTeamAsync(tournamentId, teamId, information);
            return Ok();
        }
    }
}