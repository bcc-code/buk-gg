using System;
using System.Collections.Generic;
using System.Text;

namespace Buk.Gaming.Models
{
    public class EventInfo
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Image { get; set; }

        public string CategoryId { get; set; }

        public Player Responsible { get; set; }

        public DateTime Date { get; set; }

        public string Description { get; set; }
    }
}
