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
        public Member Get(string id) => this.FirstOrDefault(i => i.PlayerId == id) ?? throw new Exception("Member not found");

        public bool Has(string id) => this.Any(i => i.PlayerId == id);
    }
}
