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
using Newtonsoft.Json.Serialization;
using Buk.Gaming.Models;
using Buk.Gaming.Providers;
using Omu.ValueInjecter;
using Buk.Gaming.Repositories;
using System.Collections.Concurrent;
using System.Threading;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Buk.Gaming.Web.Providers
{
    public class DiscordProvider : IDiscordProvider
    {
        public DiscordProvider(IConfiguration config, IHttpClientFactory clients, IOrganizationRepository organizations) {
            http = clients;
            token = config.GetValue<string>("Discord:Token");
            basePath = config.GetValue<string>("Discord:BasePath");

            _organizations = organizations;
        }
        private readonly IHttpClientFactory http;

        private string token;

        private string basePath;

        private readonly IOrganizationRepository _organizations;


        public class PostFunction
        {
            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("data")]
            public string Data { get; set; }
        }

        public async Task<DiscordUser> SyncUserAsync(Player user)
        {
            Organization[] organizations = await _organizations.GetPlayerOrganizationsAsync(user);

            var serialize = JsonConvert.SerializeObject(new { player = user, organizations }, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            var client = http.CreateClient();

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{basePath}/Sync"),
                Content = new StringContent(serialize, Encoding.UTF8, "application/json"),
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", token);


            try
            {
                var response = await client.SendAsync(request);
                var result = await response?.Content?.ReadAsStringAsync();

                if (result == null) return null;

                var obj = JsonConvert.DeserializeObject<DiscordUser>(result);

                return obj;
            } catch
            {
                return null;
            }
        }

        public async Task<List<DiscordMember>> SearchForMembersAsync(string searchString)
        {
            var client = http.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", token);
            var response = await (await client.GetAsync($"{basePath}/Search/{searchString}")).Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<DiscordMember>>(response);
        }

        public async Task<bool> IsConnectedAsync(string id)
        {
            var client = http.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", token);
            var response = await (await client.GetAsync($"{basePath}/IsConnected/{id}")).Content.ReadAsStringAsync();

            return response != "null" ? true : false;
        }
    }
}