using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Buk.Gaming.Controllers
{
    public class HomeController : Controller
    {
        public HomeController(IConfiguration configuration)
        {
            version = configuration.GetSection("Version").Value;
        }

        private string version { get; }

        public IActionResult Index()
        {
            return File("~/index.html", "text/html");
        }

        [Route("api/version")]
        public IActionResult Version()
        {
            return Ok(version);
        }
    }
}
