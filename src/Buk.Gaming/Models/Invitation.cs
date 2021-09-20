using System;
using System.Collections.Generic;
using System.Text;

namespace Buk.Gaming.Models
{
    public class Invitation
    {
        public string PlayerId { get; set; }

        public InvitationType Type { get; set; }
    }

    public class InvitationType
    {
        private InvitationType(string value)
        {
            Value = value;
        }

        public readonly string Value;

        public override string ToString() => Value;

        public static readonly InvitationType Request = new("request");
        public static readonly InvitationType Invitation = new("invitation");

        public static InvitationType Validate(string value) => value == Request.Value ? Request : Invitation;
    }
}
