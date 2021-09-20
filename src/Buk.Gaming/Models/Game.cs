using System;
using System.Collections.Generic;
using System.Text;

namespace Buk.Gaming.Models
{
    public class Game
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public bool HasTeams { get; set; }

        public string Icon { get; set; }
    }
}
