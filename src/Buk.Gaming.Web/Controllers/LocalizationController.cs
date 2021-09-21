using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buk.Gaming.Providers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Buk.Gaming.Web.Controllers
{
    [Authorize]
    [Route("api/localization")]
    public class LocalizationController : ControllerBase
    {
        public LocalizationController(ILocalizationService localization, ISessionProvider session) : base()
        {
            Localization = localization;
            Session = session;
        }

        public ILocalizationService Localization { get; }
        public ISessionProvider Session { get; }

        [Route("{lang}")]
        [HttpGet]
        public async Task<IActionResult> GetLang(string lang)
        {
            if (await Session.GetCurrentUser() == null)
            {
                return Unauthorized();
            }
            return Ok(await Localization.GetJsonAsync(lang).ConfigureAwait(false));
        }
    }
}
