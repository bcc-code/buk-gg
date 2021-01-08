using System;
using System.Collections.Generic;
using System.Text;

namespace Buk.Gaming.Toornament.Entities
{
    public class Stage
    {
        public string Id { get; set; }

        public int Number { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public bool Closed { get; set; }

        public object Settings { get; set; }
    }
}
