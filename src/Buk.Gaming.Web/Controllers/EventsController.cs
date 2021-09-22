using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buk.Gaming.Models;
using Buk.Gaming.Providers;
using Buk.Gaming.Repositories;
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
    public class EventsController : ControllerBase
    {
        public EventsController(IEventRepository eventInfo, ISessionProvider session)
        {
            EventInfo = eventInfo;
            Session = session;
        }

        public IEventRepository EventInfo { get; }
        public ISessionProvider Session { get; }

        [Route("")]
        [HttpGet]
        [ProducesDefaultResponseType(typeof(IEnumerable<EventInfo>))]
        public async Task<IActionResult> GetEventsAsync()
        {
            if (await Session.GetCurrentUser() == null)
            {
                return Unauthorized();
            }
            return Ok(await EventInfo.GetAllEventsAsync());
            // return Ok(await Toornament.Organizer.GetTournamentsAsync());
        }

        [Route("{eventId}")]
        [HttpGet]
        [ProducesDefaultResponseType(typeof(EventInfo))]
        public async Task<IActionResult> GetEventAsync(string eventId)
        {
            if (await Session.GetCurrentUser() == null)
            {
                return Unauthorized();
            }
            return Ok(await EventInfo.GetEventInfoAsync(eventId));
        }
    }
}