using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;

namespace Buk.Gaming.Controllers
{
    public class HomeController : ControllerBase
    {
        public static readonly string _version = DateTime.UtcNow.ToString();

        public IActionResult Index()
        {
            return Redirect("https://buk.gg");
        }
    }
}
