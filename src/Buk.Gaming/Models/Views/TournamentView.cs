using System;
using System.Collections.Generic;
using System.Text;

namespace Buk.Gaming.Models.Views
{
    public class TournamentView : BaseTournamentView
    {
        public string Body { get; set; }

        public string Image { get; set; }

        public bool RegistrationOpen { get; set; }

        public int MaxPlayers { get; set; }

        public int MinPlayers { get; set; }

        public List<ContactView> Contacts { get; set; }

        public TeamView Winner { get; set; }

        public List<string> RequiredInfo { get; set; }

        public List<TeamView> Teams { get; set; }

        public string TelegramLink { get; set; }

        public string ToornamentId { get; set; }

        public string Platform { get; set; }

        public string SignupType { get; set; }
    }
}
