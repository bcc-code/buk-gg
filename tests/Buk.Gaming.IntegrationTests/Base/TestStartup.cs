using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Buk.Gaming.Providers;
using Buk.Gaming.Web.Providers;
using Microsoft.AspNetCore.ResponseCompression;
using Buk.Gaming.Sanity;
using Buk.Gaming.Sanity.Serializers;
using Buk.Gaming.Images;
using Buk.Gaming.Toornament;
using Buk.Gaming.Repositories;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Buk.Gaming.Web;

namespace Buk.Gaming.IntegrationTests
{
    public class TestStartup : Startup
    {
        public TestStartup(IConfiguration configuration, IWebHostEnvironment environment) : base(configuration, environment)
        {
        }

        /// <summary>
        /// Override authentication scheme for testing purposes
        /// </summary>
        /// <param name="services"></param>
        public override void ConfigureAuthentication(IServiceCollection services)
        {
            // Add test authentication scheme, which reads claims from "X-AUTH-CLAIM" headers
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = TestAuthOptions.DefaultScheme;
                options.DefaultChallengeScheme = TestAuthOptions.DefaultScheme;
            })
            .AddScheme<TestAuthOptions, TestAuthHandler>(TestAuthOptions.DefaultScheme, o => { });

        }
    }


    public class TestAuthHandler : AuthenticationHandler<TestAuthOptions>
    {
        public TestAuthHandler(IOptionsMonitor<TestAuthOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Get test scopes
            if (!Request.Headers.TryGetValue("X-AUTH-CLAIM", out var authorization))
            {
                return Task.FromResult(AuthenticateResult.Fail("Cannot read authorization header."));
            }

            // Create authenticated user
            var identities = new List<ClaimsIdentity> { new ClaimsIdentity("TestAuth") };
            var ticket = new AuthenticationTicket(new ClaimsPrincipal(identities), Options.Scheme);

            // Add claims to test claims identity
            foreach (var header in authorization)
            {
                var parts = header.Split(':');
                identities[0].AddClaim(new Claim(parts[0], parts[1], ClaimValueTypes.String));
            }

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }

    public class TestAuthOptions : AuthenticationSchemeOptions
    {
        public const string DefaultScheme = "TestScheme";
        public string Scheme => DefaultScheme;
    }
}
