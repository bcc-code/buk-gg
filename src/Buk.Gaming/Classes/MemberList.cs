using Buk.Gaming.Extensions;
using Buk.Gaming.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buk.Gaming.Classes
{
    public class MemberList : List<Member>
    {
        public Member Get(string id) => this.FirstOrDefault(i => i.PlayerId == id) ?? throw new Exception($"Member {id} not found");

        public bool Has(string id) => this.Any(i => i.PlayerId == id);

        public MemberList WithRole(Role role) => this.Where(i => i.Role.Equals(role)).ToMemberList();

        public void SetRole(string memberId, Role role, bool unique = false)
        {
            if (unique)
            {
                foreach (var m in WithRole(role))
                {
                    m.Role = Role.Member;
                }
            }
            Get(memberId).Role = role;
        }

        public void AddMember(string id)
        {
            if (Has(id))
                throw new Exception($"Member {id} already added");

            Add(new()
            {
                PlayerId = id,
                Role = Role.Member,
            });
        }

        public void RemoveMember(string id)
        {
            Remove(Get(id));
        }

        public class UpdateOptions
        {
            public List<string> AddIds { get; set; }

            public List<string> RemoveIds { get; set; }

            public Dictionary<string, string> RoleAssignments { get; set; }
        }
    }
}
