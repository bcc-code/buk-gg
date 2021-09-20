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

        public static Role Member => new("member");

        public static Role Officer => new("officer");

        public static Role Owner => new("owner");

        public static Role Captain => new("captain");

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
    }
}
