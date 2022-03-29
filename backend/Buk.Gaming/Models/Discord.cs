using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
namespace Buk.Gaming.Models
{
    public class DiscordUser
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("tag")]
        public string Tag { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("bot")]
        public bool Bot { get; set; }

        [JsonProperty("discriminator")]
        public string Discriminator { get; set; }

        [JsonProperty("avatar")]
        public string Avatar { get; set; }

        [JsonProperty("roles")]
        public DiscordRole[] Roles { get; set; }

        [JsonProperty("created")]
        public string Created { get; set; }
    }

    public class DiscordRole
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class DiscordMember
    {
        public string userID { get; set; }
        public string displayName { get; set; }
    }
}