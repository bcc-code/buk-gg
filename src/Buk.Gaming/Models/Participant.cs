using System;
using System.Collections.Generic;
using System.Text;

namespace Buk.Gaming.Models
{
    public class Participant
    {
        public ParticipantType Type { get; set; }

        public List<string> Information { get; set; }

        public string Id { get; set; }

        public string ToornamentId { get; set; }
    }

    public class ParticipantType : IEquatable<ParticipantType>
    {
        private ParticipantType(string value)
        {
            Value = value;
        }

        public readonly string Value;

        public override string ToString() => Value;

        public static readonly ParticipantType Player = new("player");
        public static readonly ParticipantType Team = new("team");

        public static ParticipantType Validate(string value) => value == Player.Value ? Player : Team;

        public bool Equals(ParticipantType other) => this == other;

        public override bool Equals(object obj) => Equals(obj as ParticipantType);

        public override int GetHashCode() => Value.GetHashCode();

        public static bool operator ==(ParticipantType type, ParticipantType compare) => type.Value == compare.Value;
        public static bool operator !=(ParticipantType type, ParticipantType compare) => type.Value != compare.Value;
    }
}
