using Sanity.Linq.CommonTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Buk.Gaming.Sanity.Models
{
    public class SanityContact : SanityObject
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string Discord { get; set; }

        public string PhoneNumber { get; set; }
    }
}
