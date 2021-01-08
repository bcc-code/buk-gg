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
    [Route("api/session")]
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

        [Route("currentsession")]
        [HttpGet]
        public async Task<SessionDto> GetCurrentSession()
        {
            return new SessionDto
            {
                CurrentUser = await _sessionProvider.GetCurrentUser(),
                AuthenticatedUser = await _sessionProvider.GetAuthenticatedUser()
            };
        }

        [Route("currentuser")]
        [HttpPut]
        public async Task<User> UpdateCurrentUser(Player player)
        {
            player = await _players.UpdateCurrentUserAsync(player);
            var user = new User();
            user.InjectFrom(player);
            return user;
        }
    }

    public class SessionDto
    {
        public User CurrentUser { get; set; }
        public User AuthenticatedUser { get; set; }
    }

}
