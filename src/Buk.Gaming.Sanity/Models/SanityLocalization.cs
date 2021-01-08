using Sanity.Linq.CommonTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Buk.Gaming.Sanity.Models
{
    public class SanityLocalization : SanityDocument
    {
        public SanityLocalization()
        {
            SanityType = "localization";
        }

        public SanitySlug Code { get; set; }

        public string Description { get; set; }

        public string En { get; set; }

        public string No { get; set; }


        public string this[string lang]
        {
            get
            {
                if (lang?.ToLower() == "en")
                {
                    return this.En;
                }
                else if (lang?.ToLower() == "no")
                {
                    return this.No;
                }
                return En;
            }
        }
    }
}
