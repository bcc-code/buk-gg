﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Buk.Gaming.Models.Views
{
    public class MemberView
    {
        public string PlayerId { get; set; }

        public string Role { get; set; }

        public PlayerView Player { get; set; }
    }
}