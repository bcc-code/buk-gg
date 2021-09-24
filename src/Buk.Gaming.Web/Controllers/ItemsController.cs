using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buk.Gaming.Providers;
using Buk.Gaming.Repositories;
using Buk.Gaming.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Buk.Gaming.Services;
using Buk.Gaming.Extensions;
using Buk.Gaming.Models.Views;
using Microsoft.Extensions.Caching.Memory;
using Buk.Gaming.Web.Classes;

namespace Buk.Gaming.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : BaseControllerBase
    {
        private readonly IMemoryCache _cache;
        private readonly IObjectRepository _objects;

        public ItemsController(ISessionProvider session, IMemoryCache cache, IObjectRepository objects): base(session)
        {
            _cache = cache;
            _objects = objects;
        }

        [Route("Games")]
        [HttpGet]
        public async Task<IActionResult> GetGamesAsync()
        {
            return Ok(await _cache.WithSemaphoreAsync("GAMES", () => _objects.GetGamesAsync(), TimeSpan.FromMinutes(30)));
        }

        [Route("Camps")]
        [HttpGet]
        public async Task<IActionResult> GetCampsAsync()
        {
            return Ok(await _cache.WithSemaphoreAsync("CAMPS", () => _objects.GetCampsAsync(), TimeSpan.FromMinutes(30)));
        }

        [Route("Categories")]
        [HttpGet]
        public async Task<IActionResult> GetCategoriesAsync()
        {
            return Ok(await _cache.WithSemaphoreAsync("CATEGORIES", () => _objects.GetCategoriesAsync(), TimeSpan.FromMinutes(30)));
        }
    }
}
