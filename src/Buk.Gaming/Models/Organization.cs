using System.Collections.Generic;

namespace Buk.Gaming.Models
{
    public class Organization
    {
        public string Id { get; set; }

        public bool IsPublic { get; set; }

        public string Name { get; set; }

        public string Image { get; set; }

        public List<Member> Members { get; set; }

        public List<Invitation> Invitations { get; set; }

        public class CreateOptions
        {
            public string Name { get; set; }
        }
    }
}