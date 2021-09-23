using System;
using System.Collections.Generic;
using System.Text;

namespace Buk.Gaming.Models.Views
{
    public class BaseTournamentView
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Logo { get; set; }

        public string LiveStream { get; set; }

        public bool SignedUp { get; set; }

        public bool Responsible { get; set; }

        public string Slug { get; set; }
    }
}
