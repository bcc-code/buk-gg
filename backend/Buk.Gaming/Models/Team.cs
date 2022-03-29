using System.Threading.Tasks;
using System.Collections.Generic;
namespace Buk.Gaming.Models
{
    public class Team
    {
        public string Id { get; set; }

        public string Type => "team";

        public string Name { get; set; }

        public Organization Organization { get; set; }

        public Player Captain { get; set; }

        public List<Player> Players { get; set; }

        public Game Game { get; set; }
    }
}