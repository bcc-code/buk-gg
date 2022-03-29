using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Buk.Gaming.Toornament
{
    public class TournamentBase
    {
        public string Id { get; set; }

        public string Discipline { get; set; }

        public string Name { get; set; }

        public string Full_name { get; set; }

        public string Status { get; set; }

        public DateTime? Scheduled_date_start { get; set; }

        public DateTime? Scheduled_date_end { get; set; }

        public string Timezone { get; set; }

        public bool? Public { get; set; }

        public int? Size { get; set; }

        public bool? Online { get; set; }

        public string Location { get; set; }

        public string Country { get; set; }

        public string[] Platforms { get; set; }

        public object Logo { get; set; }

        public bool? Registration_enabled { get; set; }

        public DateTimeOffset? Registration_opening_datetime { get; set; }

        public DateTimeOffset? Registration_closing_datetime { get; set; }
    }
    public class Tournament : TournamentBase
    {                          

        public string Participant_type { get; set; }

        public string Organization { get; set; }

        public string Contact { get; set; }

        public string Discord { get; set; }

        public string Website { get; set; }

        public string Description { get; set; }

        public string Rules { get; set; }

        public string Prize { get; set; }

        public int? Team_size_min { get; set; }

        public int? Team_size_max { get; set; }      

        public bool? Archived { get; set; }

        public bool? Match_report_enabled { get; set; }

        public bool? Registration_notification_enabled { get; set; }

        public bool? Registration_request_message { get; set; }

        public bool? Registration_accept_message { get; set; }

        public bool? Registration_refused_message { get; set; }

        public bool? Check_in_enabled { get; set; }

        public bool? Check_in_participant_enabled { get; set; }

        public DateTime? Check_in_participant_start_datetime { get; set; }

        public DateTime? Check_in_participant_end_datetime { get; set; }

    }
}
