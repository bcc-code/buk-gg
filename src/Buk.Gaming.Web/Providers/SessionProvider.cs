using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Auth0.AuthenticationApi;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Buk.Gaming.Models;
using Buk.Gaming.Providers;
using Omu.ValueInjecter;
using Buk.Gaming.Repositories;
using System.Collections.Concurrent;
using System.Threading;

namespace Buk.Gaming.Web.Providers
{
    public class SessionProvider : ISessionProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;
        private readonly IPlayerRepository _players;
        private readonly IDiscordProvider _discord;

        private User _currentUser;
        private User _authenticatedUser;

        string[] Administrators { get; set; }

        public SessionProvider(IHttpContextAccessor httpContextAccessor, IConfiguration configuration, IMemoryCache memoryCache, IPlayerRepository players, IDiscordProvider discord)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _memoryCache = memoryCache;
            _players = players;
            _discord = discord;
            Administrators = configuration["Authorization:Admins"].Split(',').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToArray();
        }

        private static ConcurrentDictionary<string, SemaphoreSlim> _semaphores = new ConcurrentDictionary<string, SemaphoreSlim>();

        public async Task<User> GetAuthenticatedUser()
        {
            
            if (_authenticatedUser == null)
            {
                if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                {
                    string accessToken = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "access_token").Value;
                    string userId = _httpContextAccessor.HttpContext.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value;

                    if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(accessToken))
                    {
                        var semaphore = _semaphores.GetOrAdd($"AUTHENTICATED_USER_{userId}", new SemaphoreSlim(1, 1));
                        try 
                        {
                            await semaphore.WaitAsync();
                            //Cache user by userId for 5 minutes
                            _authenticatedUser = await _memoryCache.GetOrCreateAsync($"AUTHENTICATED_USER_{userId}", async u =>
                            {
                                var apiClient = new AuthenticationApiClient(_configuration["Auth0:Domain"]);
                                var userInfo = apiClient.GetUserInfoAsync(accessToken).Result;
                                var personId = userInfo.AdditionalClaims["https://login.bcc.no/claims/personId"].ToObject<int?>().GetValueOrDefault();
                                var hasMembership = userInfo.AdditionalClaims["https://login.bcc.no/claims/personId"].ToObject<bool?>().GetValueOrDefault();
                                var churchName = userInfo.AdditionalClaims["https://login.bcc.no/claims/churchName"]?.ToString() ?? "";

                                //Trying to resolve personKey from provided email address
                                var isAdmin = Administrators.Any(a => a.Equals(userInfo.Email, StringComparison.OrdinalIgnoreCase));

                                var user = new User();
                                var player = (personId > 0 ? (await _players.GetPlayerByPersonIdAsync(personId)) : null) ?? (await _players.GetPlayerAsync(userInfo.Email));
                                if (!hasMembership || personId == 0)
                                {
                                    if (player != null)
                                    {
                                        await _players.DeletePlayerAsync(player.Id);
                                        user = null;
                                    }
                                }
                                else
                                {
                                    if (player == null || player?.DateRegistered == null)
                                    {
                                        player = new Player { Email = userInfo.Email, DateRegistered = DateTimeOffset.Now };
                                    }
                                    
                                    var date = DateTime.Now.AddYears(-18);
                                    var playerDate = DateTime.Parse(userInfo.Birthdate);

                                    player.Name = userInfo.FullName;
                                    player.NoNbIsStandard = userInfo.Locale == "nb-NO" ? true : false;
                                    player.IsO18 = date > playerDate ? true : false; 
                                    player.Nickname = player.Nickname ?? userInfo.FirstName;
                                    player.DateLastActive = DateTimeOffset.Now;
                                    player.PersonId = personId;
                                    player.Location = churchName ?? player.Location;

                                    if (player.DiscordId != null) {
                                        var discordUser = await _discord.SyncUserAsync(player);
                                        if (discordUser.Id != null) {
                                            player.DiscordIsConnected = true;
                                            player.DiscordUser = discordUser.Tag;
                                        }
                                    }
                                    if (player.Email != userInfo.Email)
                                    {
                                        var existingWithSameEmail = await _players.GetPlayerAsync(userInfo.Email);
                                        if (existingWithSameEmail != null)
                                        {
                                            await _players.DeletePlayerAsync(existingWithSameEmail.Id);
                                        }
                                        player.Email = userInfo.Email;
                                    }
                                    player = await _players.SavePlayerAsync(player);
                                    user.InjectFrom(player);
                                    user.CanImpersonate = isAdmin;
                                    user.IsAdministrator = isAdmin;
                                }
            
                                u.Priority = CacheItemPriority.High;
                                u.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));


                                return user;
                            });

                        } 
                        finally
                        {
                            semaphore.Release();
                        }
                    }
                    
                }
            }
            return _authenticatedUser;
            
        }

        public async Task<User> GetCurrentUser()
        {            
            if (_currentUser == null)
            {
                //TODO: Allow only if original user has permission to impersonate
                var impersonateEmailHeader = _httpContextAccessor.HttpContext?.Request.Headers["X-Impersonate-Email"];
                var impersonateEmail = HttpUtility.UrlDecode(impersonateEmailHeader?.ToString());
                var authenticatedUser = await GetAuthenticatedUser();
                if (!string.IsNullOrEmpty(impersonateEmail) && authenticatedUser.CanImpersonate)
                {
                    _currentUser = await _memoryCache.GetOrCreateAsync($"USER_{impersonateEmail}", async u =>
                    {
                        u.Priority = CacheItemPriority.High;
                        u.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

                        var player = await _players.GetPlayerAsync(impersonateEmail);
                        if (player == null)
                        {
                            player = await _players.SavePlayerAsync(new Player { Email = impersonateEmail });
                        }
                        var user = new User();
                        user.InjectFrom(player);
                        return user;


                    });
                }
                else _currentUser = authenticatedUser;
            }
            return _currentUser;
            
        }

        
    }
}