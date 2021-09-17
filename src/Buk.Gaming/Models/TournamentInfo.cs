using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Buk.Gaming.Models
{
    public class TournamentInfo
    {
        public string Id { get; set; }

        public string Slug { get; set;}

        public string Title { get; set; }

        public string Platform { get; set; }

        public string ResponsibleId { get; set; }

        public List<string> CategoryIds { get; set; }

        public string MainImage { get; set; }

        public string Logo { get; set; }

        public string Introduction { get; set;}

        public string Body { get; set;}

        public string RegistrationForm { get; set; }

        public bool RegistrationOpen { get; set; }

        public string ToornamentId { get; set; }

        public string TelegramLink { get; set; }

        public string LiveStream { get; set; }

        public Game Game { get; set; }

        public List<Team> Teams { get; set; } = new List<Team>();

        public string SignupType { get; set; }

        public List<string> RequiredInformation { get; set; }

        public List<string> PlayerIds { get; set; }

        public string Winner { get; set; }

        public TeamSize TeamSize { get; set; }

        public List<Contact> Contacts { get; set; } = new List<Contact>();
                
        public bool LiveChat { get; set; }
    }

    public class TournamentAdminInfo : TournamentInfo {
        public Player Responsible { get; set; }

        public List<Participant<Player>> SoloPlayers { get; set; } = new List<Participant<Player>>();

        public List<Participant<Team>> ParticipantTeams { get; set; } = new List<Participant<Team>>();
    }

    public class TeamSize
    {
        public int Max { get; set; }

        public int Min { get; set; }
    }

    public class Contact {
        [JsonProperty("_key")]
        public string Key { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Discord { get; set; }

        public string PhoneNumber { get; set; }
    }

    public class Game
    {
        [JsonProperty("_id")]
        public string Id { get; set; }

        public string Name { get; set; }

        public bool HasTeams { get; set; }

        public string Icon { get; set; }
    }

    public class Participant {
        public string Type { get; set; }

        public List<string> Information { get; set; }

        public string Id { get; set; }

        public string ToornamentId { get; set; }
    }
}
