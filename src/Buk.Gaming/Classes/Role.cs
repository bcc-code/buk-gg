using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Buk.Gaming.Classes
{
    public class Role : IEquatable<Role>
    {
        private Role(string role)
        {
            Value = role;
        }

        public readonly string Value;

        public int Strength => _strengths.GetValueOrDefault(this);

        public static readonly Role Member = new("member");

        public static readonly Role Officer = new("officer");

        public static readonly Role Owner = new("owner");

        public static readonly Role Captain = new("captain");

        private readonly static Dictionary<Role, int> _strengths = new()
        {
            [Member] = 1,
            [Captain] = 2,
            [Officer] = 3,
            [Owner] = 4,
        };

        public override string ToString() => Value;

        public override bool Equals(object obj) => Equals(obj as Role);

        public bool Equals(Role role) => Value == role.Value;

        public override int GetHashCode() => Value.GetHashCode();

        public static Role[] All => new Role[] { Member, Officer, Owner };

        public static Role Validate(string value, bool strict = true)
        {
            var role = All.FirstOrDefault(r => r.Value == value);

            if (strict && role == null)
            {
                throw new Exception("Role not validated");
            }
            return new Role(value);
        }

        public static bool operator >=(Role role, Role compare) => role.Strength >= compare.Strength;
        public static bool operator <=(Role role, Role compare) => role.Strength <= compare.Strength;
        public static bool operator <(Role role, Role compare) => role.Strength < compare.Strength;
        public static bool operator >(Role role, Role compare) => role.Strength > compare.Strength;

        public void VerifyLessThan(Role role) 
        {
            if (this >= role)
                throw new Exception("Role is greater than specified");
        }
    }
}
