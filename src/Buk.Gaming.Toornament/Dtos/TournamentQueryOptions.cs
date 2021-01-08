using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Buk.Gaming.Toornament.Dtos
{
    public class TournamentQueryOptions
    {
        public string Disciplines { get; set; }

        public string Statuses { get; set; }

        public DateTime Scheduled_before { get; set; }

        public DateTime Scheduled_after { get; set; }

        public string Countries { get; set; }

        public string Platforms { get; set; }

        public bool Is_online { get; set; }

        public string Custom_user_identifier { get; set; }

        public string Sort { get; set; }

    }
}
