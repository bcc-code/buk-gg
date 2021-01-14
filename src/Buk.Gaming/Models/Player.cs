using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Buk.Gaming.Models
{
    public class Player
    {
        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("_type")]
        public string Type => "player";

        [JsonIgnore]
        public int PersonId { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string Name { get; set; }

        public string Nickname { get; set; }

        public string DisplayName => Nickname ?? Name ?? Email ?? "";

        public string Location { get; set; }
    
        public bool NoNbIsStandard { get; set; }

        public bool IsO18 { get; set; }

        public string DiscordUser { get; set; }

        public string DiscordId { get; set; }
        
        public bool DiscordIsConnected { get; set; }

        [JsonIgnore]
        public bool EnableMoreDiscords { get; set; }

        public ExtraDiscordUser[] MoreDiscordUsers { get; set; }

        [JsonIgnore]
        public DateTimeOffset DateRegistered { get; set; }

        [JsonIgnore]
        public DateTimeOffset DateLastActive { get; set; }

        public bool IsRegistered => DateRegistered != null;
    }

    public class ExtraDiscordUser
    {
        [JsonProperty("_key")]
        public string Key { get; set; }

        public string Name { get; set; }

        public string DiscordId { get; set; }
    }
}
