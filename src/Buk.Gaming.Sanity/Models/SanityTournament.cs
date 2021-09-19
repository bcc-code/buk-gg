using Buk.Gaming.Models;
using Sanity.Linq;
using Newtonsoft.Json;
using Sanity.Linq.BlockContent;
using Sanity.Linq.CommonTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Sanity.Linq.Extensions;

namespace Buk.Gaming.Sanity.Models
{
    public class SanityTournament : SanityDocument
    {
        public SanityTournament()
        {
            SanityType = "tournament";
        }

        public SanityLocaleString Title { get; set;}

        public SanitySlug Slug { get; set; }

        public SanityReference<SanityCamp> Camp { get; set; }

        public string Platform { get; set; }

        public List<SanityReference<SanityCategory>> Categories { get; set; }

        public SanityReference<Player> Responsible { get; set; }

        [Include]
        public SanityImage MainImage { get; set; }

        [Include]
        public SanityImage Logo { get; set; }

        public SanityLocaleBlock Introduction { get; set; }

        public SanityLocaleBlock Body { get; set; }

        public SanityLocaleString   RegistrationForm { get; set;}

        public bool RegistrationOpen { get; set; }

        public string ToornamentId { get; set; }

        public string TelegramLink { get; set; }

        public string LiveStream { get; set; }

        public bool LiveChat { get; set; }
        
        public SanityReference<SanityGame> Game { get; set; }

        public string SignupType { get; set; }

        public SanityReference<SanityTeam> Winner { get; set; }

        public List<SanityLocaleString> RequiredInfo { get; set; }

        public List<SanityParticipant> SoloPlayers { get; set; }

        public List<SanityParticipant> Teams { get; set; }

        public TeamSize TeamSize { get; set; }

        public List<SanityContact> Contacts { get; set; }

    }
}
