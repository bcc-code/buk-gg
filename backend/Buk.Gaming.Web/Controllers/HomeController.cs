using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;

namespace Buk.Gaming.Web.Controllers
{
    public class HomeController : Controller
    {
        public static readonly string _version = DateTime.UtcNow.ToString();

        public IActionResult Index()
        {
            return Redirect("https://buk.gg");
        }

        [Route("api/version")]
        public IActionResult Version()
        {
            return Ok(_version);
        }
    }
}
