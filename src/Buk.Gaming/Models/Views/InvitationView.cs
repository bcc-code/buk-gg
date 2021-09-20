using System;
using System.Collections.Generic;
using System.Text;

namespace Buk.Gaming.Models.Views
{
    public class InvitationView
    {
        public string PlayerId { get; set; }

        public string Type { get; set; }

        public PlayerView Player { get; set; }
    }
}
