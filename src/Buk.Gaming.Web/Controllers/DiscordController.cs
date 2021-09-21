using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buk.Gaming.Providers;
using Buk.Gaming.Models;
using Buk.Gaming.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace Buk.Gaming.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DiscordController : ControllerBase
    {
        public DiscordController(ISessionProvider session, IDiscordProvider discord)
        {
            Session = session;
            Discord = discord;
        }

        public ISessionProvider Session { get; }

        public IDiscordProvider Discord { get; }

        [Route("Search/{searchString}")]
        [HttpGet]
        public async Task<IActionResult> SearchForMembers(string searchString)
        {
            User user = await Session.GetCurrentUser();
            if (user == null) 
            {
                return Unauthorized();
            }

            var result = await Discord.SearchForMembersAsync(searchString);

            return Ok(result);
        }
    }
}
