using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;

namespace Buk.Gaming.Controllers
{
    public class HomeController : Controller
    {
        public static readonly string _version = DateTime.UtcNow.ToString();

        public IActionResult Index()
        {
            return File("~/index.html", "text/html");
        }

        [Route("api/version")]
        public IActionResult Version()
        {
            return Ok(_version);
        }
    }
}
