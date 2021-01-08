using Sanity.Linq.BlockContent;
using Sanity.Linq.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Buk.Gaming.Sanity.Models
{
    public class SanityLocaleString : Dictionary<string, string>
    {
        public SanityLocaleString() : base()
        {
        }

        public SanityLocaleString(IDictionary<string, string> dictionary) : base(dictionary)
        {
        }

        public SanityLocaleString(IReadOnlyDictionary<string, string> dictionary) : base(dictionary.ToDictionary(kv => kv.Key, kv => kv.Value))
        {
        }

        public SanityLocaleString(Dictionary<string, string> dictionary) : base(dictionary)
        {

        }

        public SanityLocaleString Set(string languageCode, string value)
        {
            this[languageCode] = value;
            return this;
        }

        public string GetForCurrentCulture()
        {
            // Current thread culture
            var lang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            if (lang == "nb" || lang == "nn") lang = "no";

            string val = null;
            if (this.ContainsKey(lang)) val = this[lang];

            // Default culture
            if (string.IsNullOrEmpty(val) && CultureInfo.DefaultThreadCurrentUICulture != null)
            {
                lang = CultureInfo.DefaultThreadCurrentUICulture.TwoLetterISOLanguageName;
                if (lang == "nb" || lang == "nn") lang = "no";
                if (this.ContainsKey(lang)) val = this[lang];
            }

            // English
            if (string.IsNullOrEmpty(val))
            {
                lang = "en";
                if (this.ContainsKey(lang)) val = this[lang];
            }

            // German
            if (string.IsNullOrEmpty(val))
            {
                lang = "de";
                if (this.ContainsKey(lang)) val = this[lang];
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
