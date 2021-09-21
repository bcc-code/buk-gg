using System;
using System.Collections.Generic;
using System.Text;

namespace Buk.Gaming.Models.Views
{
    public class OrganizationView : BaseItemView
    {
        public List<TeamView> Teams { get; set; }

        public List<MemberView> Members { get; set; }

        public List<InvitationView> Invitations { get; set; }

        public string Logo { get; set; }
    }
}
