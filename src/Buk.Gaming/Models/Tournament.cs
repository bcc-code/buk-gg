using Buk.Gaming.Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Buk.Gaming.Models
{
    public class Tournament
    {
        public string Id { get; set; }

        public string Slug { get; set;}

        public LocaleDictionary Title { get; set; }

        public string Platform { get; set; }

        public string ResponsibleId { get; set; }

        public List<string> CategoryIds { get; set; }

        public string MainImage { get; set; }

        public string Logo { get; set; }

        public LocaleDictionary Introduction { get; set;}

        public LocaleDictionary Body { get; set;}

        public bool RegistrationOpen { get; set; }

        public string ToornamentId { get; set; }

        public string TelegramLink { get; set; }

        public string LiveStream { get; set; }

        public string GameId { get; set; }

        public List<string> TeamIds { get; set; }

        public string SignupType { get; set; }

        public List<LocaleDictionary> RequiredInformation { get; set; }

        public List<string> PlayerIds { get; set; }

        public string Winner { get; set; }

        public TeamSize TeamSize { get; set; }

        public List<Contact> Contacts { get; set; } = new List<Contact>();

        public bool LiveChat { get; set; }
    }

    public class TeamSize
    {
        public int Max { get; set; }

        public int Min { get; set; }
    }
}
