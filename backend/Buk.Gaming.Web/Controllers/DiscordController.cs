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


namespace Buk.Gaming.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DiscordController : Controller
    {
        public DiscordController(ISessionProvider session, IDiscordProvider discord)
        {
            Session = session;
            Discord = discord;
        }

        public ISessionProvider Session { get; }

        public IDiscordProvider Discord { get; }
    }
}
