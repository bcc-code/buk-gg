using Microsoft.AspNetCore.Mvc;
using Buk.Gaming.Models;
using Buk.Gaming.Providers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Omu.ValueInjecter;
using Microsoft.AspNetCore.Authorization;
using Buk.Gaming.Services;

namespace Buk.Gaming.Controllers
{
    [Authorize]
    [Route("api/Session")]
    [ApiController]
    public class SessionController : ControllerBase
    {
        private readonly ISessionProvider _sessionProvider;
        private readonly IPlayerService _players;

        public SessionController(ISessionProvider sessionProvider, IPlayerService players)
        {
            _sessionProvider = sessionProvider;
            _players = players;
        }

        [Route("")]
        [HttpGet]
        [ProducesDefaultResponseType(typeof(User))]
        public async Task<IActionResult> GetCurrentSession()
        {
            return Ok(await _sessionProvider.GetCurrentUser());
        }

        [Route("")]
        [HttpPut]
        [ProducesDefaultResponseType(typeof(User))]
        public async Task<User> UpdateCurrentUser([FromBody] Player.UpdateOptions options)
        {
            var player = await _sessionProvider.GetCurrentUser();

            await _players.UpdatePlayerAsync(player, options);

            return player;
        }
    }
}
