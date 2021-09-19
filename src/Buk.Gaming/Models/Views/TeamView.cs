using System;
using System.Collections.Generic;
using System.Text;

namespace Buk.Gaming.Models.Views
{
    public class TeamView
    {
        public string Id { get; set; }

        public string OrganizationId { get; set; }

        public string Name { get; set; }

        public string CaptainId { get; set; }

        public string[] PlayerIds { get; set; }

        public List<PlayerView> Members { get; set; }
    }
}
