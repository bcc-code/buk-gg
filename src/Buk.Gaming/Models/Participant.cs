using System;
using System.Collections.Generic;
using System.Text;

namespace Buk.Gaming.Models
{
    public class Participant
    {
        public ParticipantType Type { get; set; }

        public List<string> Information { get; set; }

        public string Id { get; set; }

        public string ToornamentId { get; set; }
    }

    public enum ParticipantType
    {
        Player,
        Team
    }
}
