using System;
using System.Collections.Generic;
using System.Text;

namespace Buk.Gaming.Toornament
{
    public static class ToornamentScopes
    {
        public static class Participant
        {
            public const string ManageRegistrations = "participant:manage_registrations";
            public const string ManageParticipants = "participant:manage_participations";
            public const string All = "participant:manage_registrations participant:manage_participations";
        }

        public static class User
        {
            public const string Info = "user:info";
            public const string All = "participant:manage_registrations participant:manage_participations";
        }

        public static class Organizer
        {
            public const string View = "organizer:view";
            public const string Admin = "organizer:admin";
            public const string Result = "organizer:result";
            public const string Participant = "organizer:participant";
            public const string Registration = "organizer:registration";
            public const string Delete = "organizer:delete";
            public const string All = "organizer:view organizer:admin organizer:result organizer:participant organizer:registration organizer:delete";
        }
        
    }
}
