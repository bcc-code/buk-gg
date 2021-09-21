using Buk.Gaming.Classes;
using System.Collections.Generic;

namespace Buk.Gaming.Models
{
    public class Organization
    {
        public string Id { get; set; }

        public bool IsPublic { get; set; }

        public string Name { get; set; }

        public string Image { get; set; }

        public MemberList Members { get; set; }

        public List<Invitation> Invitations { get; set; }

        public class CreateOptions
        {
            public string Name { get; set; }
        }

        public class UpdateOptions
        {
            public string Name { get; set; }

            public string Image { get; set; }
        }

        public class MemberOptions
        {
            public List<string> AddMembers { get; set; }

            public List<string> RemoveMembers { get; set; }

            public Dictionary<string, string> RoleAssignments { get; set; }
        }
    }
}