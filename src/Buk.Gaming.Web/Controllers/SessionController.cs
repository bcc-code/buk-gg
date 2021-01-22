using Microsoft.AspNetCore.Mvc;
using Buk.Gaming.Models;
using Buk.Gaming.Providers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Omu.ValueInjecter;
using Microsoft.AspNetCore.Authorization;

namespace Buk.Gaming.Controllers
{
    [Authorize]
    [Route("api/Session")]
    [ApiController]
    public class SessionController : ControllerBase
    {
        private readonly ISessionProvider _sessionProvider;
        private readonly PlayerService _players;

        public SessionController(ISessionProvider sessionProvider, PlayerService players)
        {
            this._sessionProvider = sessionProvider;
            this._players = players;
        }

        [Route("")]
        [HttpGet]
        public async Task<IActionResult> GetCurrentSession()
        {
            return Ok(await _sessionProvider.GetCurrentUser());
        }

        [Route("")]
        [HttpPut]
        public async Task<User> UpdateCurrentUser(Player player)
        {
            player = await _players.UpdateCurrentUserAsync(player);
            var user = new User();
            user.InjectFrom(player);
            return user;
        }
    }
}
