using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Buk.Gaming.Classes
{
    public class LocaleDictionary : Dictionary<string, string>
    {
        public string GetForCurrentCulture()
        {
            // Current thread culture
            var lang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            if (lang == "nb" || lang == "nn") lang = "no";

            string val = null;
            if (ContainsKey(lang)) val = this[lang];

            // Default culture
            if (string.IsNullOrEmpty(val) && CultureInfo.DefaultThreadCurrentUICulture != null)
            {
                lang = CultureInfo.DefaultThreadCurrentUICulture.TwoLetterISOLanguageName;
                if (lang == "nb" || lang == "nn") lang = "no";
                if (ContainsKey(lang)) val = this[lang];
            }

            // English
            if (string.IsNullOrEmpty(val))
            {
                lang = "en";
                if (ContainsKey(lang)) val = this[lang];
            }

            // German
            if (string.IsNullOrEmpty(val))
            {
                lang = "de";
                if (ContainsKey(lang)) val = this[lang];
            }

            // First non-empty
            if (string.IsNullOrEmpty(val))
            {
                val = this.Select(kv => kv.Value).FirstOrDefault(v => v != null && !(v is string));
            }

            return val;

        }
    }
}
