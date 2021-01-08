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

namespace Buk.Gaming.Web.Providers
{
    public class DiscordProvider : IDiscordProvider
    {
        public DiscordProvider(IConfiguration config, IHttpClientFactory clients) {
            http = clients;
            this.token = config.GetValue<string>("Discord:Token");
        }
        private readonly IHttpClientFactory http;

        private string token;

        private string basePath = "https://discord.buk.gg"; //https://discord.buk.gg


        public class PostFunction
        {
            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("data")]
            public string Data { get; set; }
        }

        public async Task<DiscordUser> GetUserAsync(string id)
        {
            var client = http.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", token);
            var response = await client.GetAsync($"{basePath}/Get/{id}");
            return JsonConvert.DeserializeObject<DiscordUser>(await response.Content.ReadAsStringAsync());
        }

        public async Task<DiscordUser> UpdateUserAsync(User user)
        {
            var serialize = JsonConvert.SerializeObject(user, new JsonSerializerSettings 
            { 
                ContractResolver = new CamelCasePropertyNamesContractResolver() 
            });
            
            var client = http.CreateClient();
            var content = new StringContent(serialize, System.Text.Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{basePath}/Update"),
                Content = content,
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", token);
            var response = await client.SendAsync(request);
            var result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<DiscordUser>(result);
        }
        public async Task<dynamic> SearchForMembers(string searchString)
        {
            var client = http.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", token);
            var response = await (await client.GetAsync($"{basePath}/Search/{searchString}")).Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject(response);
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