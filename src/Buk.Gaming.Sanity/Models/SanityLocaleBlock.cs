using Sanity.Linq.BlockContent;
using Sanity.Linq.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Buk.Gaming.Sanity.Models
{
    public class SanityLocaleBlock : Dictionary<string, object>
    {
        public SanityLocaleBlock() : base()
        {
        }

        public SanityLocaleBlock(IDictionary<string, object> dictionary) : base(dictionary)
        {
        }

        public SanityLocaleBlock(IReadOnlyDictionary<string, object> dictionary) : base(dictionary.ToDictionary(kv => kv.Key, kv => kv.Value))
        {
        }

        public SanityLocaleBlock(Dictionary<string, object> dictionary) : base(dictionary)
        {

        }

        public SanityLocaleBlock Set(string languageCode, object value)
        {
            this[languageCode] = value;
            return this;
        }

        public string GetForCurrentCulture(SanityHtmlBuilder sanityHtmlBuilder)
        {
            // Current thread culture
            var lang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            if (lang == "nb" || lang == "nn") lang = "no";

            string val = null;
            if (this.ContainsKey(lang)) val = this[lang].ToHtml(sanityHtmlBuilder);

            // Default culture
            if (string.IsNullOrEmpty(val) && CultureInfo.DefaultThreadCurrentUICulture != null)
            {
                lang = CultureInfo.DefaultThreadCurrentUICulture.TwoLetterISOLanguageName;
                if (lang == "nb" || lang == "nn") lang = "no";
                if (this.ContainsKey(lang)) val = this[lang].ToHtml(sanityHtmlBuilder);
            }

            // English
            if (string.IsNullOrEmpty(val))
            {
                lang = "en";
                if (this.ContainsKey(lang)) val = this[lang].ToHtml(sanityHtmlBuilder);
            }

            // German
            if (string.IsNullOrEmpty(val))
            {
                lang = "de";
                if (this.ContainsKey(lang)) val = this[lang].ToHtml(sanityHtmlBuilder);
            }

            // First non-empty
            if (string.IsNullOrEmpty(val))
            {
                val = this.Select(kv => kv.Value).FirstOrDefault(v => v != null && !(v is string))?.ToHtml(sanityHtmlBuilder);
            }

            return val;

        }
    }
}
