using Buk.Gaming.Classes;
using Buk.Gaming.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buk.Gaming.Extensions
{
    public static class Conversions
    {
        public static MemberList ToMemberList(this IEnumerable<Member> i)
        {
            MemberList list = new();
            foreach (Member m in i)
            {
                list.Add(m);
            }
            return list;
        }
    }
}
