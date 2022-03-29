using Newtonsoft.Json.Linq;
using Sanity.Linq.CommonTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Buk.Gaming.Sanity.Models
{
    public class SanityLocalizationBlock : SanityDocument
    {
        public SanityLocalizationBlock()
        {
            SanityType = "localizationBlock";
        }

        public SanitySlug Code { get; set; }

        public string Description { get; set; }

        public JObject[] En { get; set; }

        public JObject[] No { get; set; }

        public JObject[] this[string lang]
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
