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

    public enum InvitationType
    {
        Request,
        Invitation
    }
}
