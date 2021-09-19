using Buk.Gaming.Models;
using Sanity.Linq;
using Sanity.Linq.BlockContent;
using Sanity.Linq.CommonTypes;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Buk.Gaming.Sanity.Models {
    public class SanityOrganization : SanityDocument
    {
        public SanityOrganization()
        {
            SanityType = "organization";
        }

        public string Name { get; set; }

        public bool IsPublic { get; set; }

        public string DiscordRoleId { get; set; }

        public List<SanityMember> Members { get; set; }

        public List<SanityInvitation> Pending { get; set; }

        [Include]
        public SanityImage Image { get; set; }
    }
}