using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Buk.Gaming.Models
{
    public class Team
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string OrganizationId { get; set; }

        public string GameId { get; set; }

        public string CaptainId { get; set; }

        public List<string> PlayerIds { get; set; }

        public class CreateOptions
        {
            public string Name { get; set; }

            public string OrganizationId { get; set; }

            public string CaptainId { get; set; }

            public string GameId { get; set; }
        }
    }
}