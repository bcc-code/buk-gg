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

        public TournamentsController(ISessionProvider session, ToornamentClient toornament, ITournamentService tournaments): base(session)
        {
            _toornamentClient = toornament;
            _tournaments = tournaments;
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

            var participants = await _tournaments.GetParticipantsAsync(tournamentId);

            return Ok(participants.participants.Select(p => p.View(participants.players.FirstOrDefault(i => i.Id == p.Id))).ToList());
        }
    }
}