using System;
using System.Collections.Generic;
using System.Text;

namespace Buk.Gaming.Models.Views
{
    public class TournamentView
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }

        public bool RegistrationOpen { get; set; }

        public string LiveStream { get; set; }

        public int MaxPlayers { get; set; }

        public int MinPlayers { get; set; }

        public List<ContactView> Contacts { get; set; }

        public TeamView Winner { get; set; }

        public string SignupType { get; set; }

        public List<string> RequiredInfo { get; set; }

        public List<TeamView> Teams { get; set; }
    }
}
