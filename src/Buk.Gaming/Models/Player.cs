using System;
using System.Collections.Generic;
using System.Text;

namespace Buk.Gaming.Models
{
    public class Player
    {
        public string Id { get; set; }

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

        public bool EnableMoreDiscords { get; set; }

        public List<ExtraDiscordUser> MoreDiscordUsers { get; set; }

        public DateTimeOffset DateRegistered { get; set; }

        public DateTimeOffset DateLastActive { get; set; }

        public bool IsRegistered => DateRegistered != default;

        public class UpdateOptions
        {
            public string Nickname { get; set; }

            public string PhoneNumber { get; set; }

            public string DiscordId { get; set; }

            public string DiscordUser { get; set; }
        }

        public class ExtraDiscordUser
        {
            public string Name { get; set; }

            public string DiscordId { get; set; }
        }
    }
}
