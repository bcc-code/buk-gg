using Sanity.Linq.CommonTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Buk.Gaming.Sanity.Models
{
    public class SanityParticipant
    {
        public List<string> Information { get; set; }

        public SanityReference<SanityTeam> Team { get; set; }

        public SanityReference<SanityPlayer> Player { get; set; }

        public string ToornamentId { get; set; }
    }
}
