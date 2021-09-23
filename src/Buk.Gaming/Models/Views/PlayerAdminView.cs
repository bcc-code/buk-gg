using System;
using System.Collections.Generic;
using System.Text;

namespace Buk.Gaming.Models.Views
{
    public class PlayerAdminView : PlayerView
    {
        public string Name { get; set; }

        public string PhoneNumber { get; set; }

        public string Location { get; set; }

        public string Email { get; set; }
    }
}
