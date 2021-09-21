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

        public SignupType SignupType { get; set; }

        public List<LocaleDictionary> RequiredInformation { get; set; }

        public string WinnerId { get; set; }

        public TeamSize TeamSize { get; set; }

        public List<Contact> Contacts { get; set; } = new List<Contact>();

        public bool LiveChat { get; set; }

        public List<Participant> Participants { get; set; }
    }

    public class TeamSize
    {
        public int Max { get; set; }

        public int Min { get; set; }
    }

    public class SignupType : IEquatable<SignupType>
    {
        private SignupType(string value)
        {
            Value = value;
        }

        public readonly string Value;

        public static readonly SignupType Player = new("player");
        public static readonly SignupType Team = new("team");
        public static readonly SignupType None = new("none");

        public override string ToString() => Value;

        public static SignupType Validate(string value) => value == Player.ToString() ? Player : Team;

        public bool Equals(SignupType other) => other.Value == Value;

        public override bool Equals(object obj) => Equals(obj as SignupType);
    }
}
