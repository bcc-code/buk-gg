using Buk.Gaming.Providers;
using Buk.Gaming.Sanity.Models;
using Microsoft.Extensions.Caching.Memory;
using Sanity.Linq;
using Sanity.Linq.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Buk.Gaming.Sanity
{

    public class CachedSanityLocalizationService : SanityLocalizationService
    {
        private IMemoryCache _cache { get; }

        public CachedSanityLocalizationService(SanityDataContext sanity, SanityOptions options, IMemoryCache cache) : base(sanity, options)
        {
            _cache = cache;
        }


        public override Task<object> GetJsonAsync(string lang, string module = "")
        {
            return _cache.GetOrCreateAsync("SANITY_LOCALIZATION_" + (lang ?? "") + "_" + (module ?? ""), (c) =>
            {
                c.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60);
                return base.GetJsonAsync(lang, module);
            });
        }
    }

    public class SanityLocalizationService : ILocalizationService
    {
        private SanityDataContext _sanity { get; }

        public SanityLocalizationService(SanityDataContext sanity, SanityOptions options)
        {
            _sanity = sanity;
        }

        public virtual async Task<object> GetJsonAsync(string lang, string module = "")
        {
            var localizations = await GetLocalizationsAsync().ConfigureAwait(false);
            if (localizations.ContainsKey(lang.ToLower()))
            {
                var languageLocalizations = localizations[lang.ToLower()];

                var root = new Dictionary<string, object>();
                foreach (var loc in languageLocalizations)
                {
                    var code = loc.Key;
                    var parts = code.Split('.');
                    Dictionary<string, object> currentNode = root;
                    for (int i = 0; i < parts.Length - 1; i++)
                    {
                        if (!currentNode.ContainsKey(parts[i]) || !(currentNode[parts[i]] is Dictionary<string, object>))
                        {
                            currentNode[parts[i]] = new Dictionary<string, object>();
                        }
                        currentNode = currentNode[parts[i]] as Dictionary<string, object>;
                    }
                    currentNode[parts[^1]] = loc.Value;
                }

                return root;
            }

            // Default: empty object
            return new object();
        }

        public async Task<Dictionary<string, Dictionary<string, string>>> GetLocalizationsAsync()
        {
            var localizations = await _sanity.DocumentSet<SanityLocalization>().ToListAsync().ConfigureAwait(false);
            var localizationBlocks = await _sanity.DocumentSet<SanityLocalizationBlock>().Where(d => !d.IsDraft()).ToListAsync().ConfigureAwait(false);
            var pages = await _sanity.DocumentSet<SanityPage>().ToListAsync().ConfigureAwait(false);

            var result = new Dictionary<string, Dictionary<string, string>>
            {
                ["en"] = new Dictionary<string, string>(),
                ["no"] = new Dictionary<string, string>()
            };


            // Localization Strings //
            foreach (var localization in localizations)
            {
                foreach (var lang in result.Keys)
                {
                    var code = (localization.Code?.Current ?? "").Trim();
                    if (code != "")
                    {
                        var value = localization[lang];
                        if (string.IsNullOrEmpty(value))
                        {
                            value = localization["no"];
                        }
                        if (string.IsNullOrEmpty(value))
                        {
                            value = localization["en"];
                        }
                        result[lang][code] = value;
                    }
                }
            }


            // Localization Blocks //
            foreach (var localization in localizationBlocks)
            {
                foreach (var lang in result.Keys)
                {
                    var code = (localization.Code?.Current ?? "").Trim();
                    if (code != "")
                    {
                        var value = localization[lang]?.ToHtml(_sanity);
                        if (string.IsNullOrEmpty(value))
                        {
                            value = localization["no"]?.ToHtml(_sanity);
                        }
                        if (string.IsNullOrEmpty(value))
                        {
                            value = localization["en"]?.ToHtml(_sanity);
                        }
                        result[lang][code] = value ?? "";
                    }
                }
            }

            // Pages //
            foreach (var page in pages)
            {
                var prefix = page?.Slug?.Current;
                if (string.IsNullOrEmpty(prefix))
                {
                    continue;
                }
                foreach (var lang in result.Keys)
                {
                    // Title
                    if (page.Title != null)
                    {
                        var code = $"{prefix}.title";
                        var value = page.Title.ContainsKey(lang) ? page.Title[lang] : null;
                        if (string.IsNullOrEmpty(value) && page.Title.ContainsKey("no"))
                        {
                            value = page.Title["no"];
                        }
                        if (string.IsNullOrEmpty(value) && page.Title.ContainsKey("en"))
                        {
                            value = page.Title["en"];
                        }
                        result[lang][code] = value ?? "";
                    }

                    // Introduction
                    if (page.Introduction != null)
                    {
                        var code = $"{prefix}.introduction";
                        var value = page.Introduction.ContainsKey(lang) ? page.Introduction[lang].ToHtml(_sanity) : null;
                        if (string.IsNullOrEmpty(value) && page.Introduction.ContainsKey("no"))
                        {
                            value = page.Introduction["no"].ToHtml(_sanity);
                        }
                        if (string.IsNullOrEmpty(value) && page.Introduction.ContainsKey("en"))
                        {
                            value = page.Introduction["en"].ToHtml(_sanity);
                        }
                        result[lang][code] = value ?? "";
                    }

                    // Body
                    if (page.Body != null)
                    {
                        var code = $"{prefix}.body";
                        var value = page.Body.ContainsKey(lang) ? page.Body[lang].ToHtml(_sanity) : null;
                        if (string.IsNullOrEmpty(value) && page.Body.ContainsKey("no"))
                        {
                            value = page.Body["no"].ToHtml(_sanity);
                        }
                        if (string.IsNullOrEmpty(value) && page.Body.ContainsKey("en"))
                        {
                            value = page.Body["en"].ToHtml(_sanity);
                        }
                        result[lang][code] = value ?? "";
                    }

                    // MainImage
                    if (!string.IsNullOrEmpty(page.MainImage?.Asset?.Value?.Url)) 
                    {
                        var code = $"{prefix}.mainImage";
                        result[lang][code] = page.MainImage?.Asset?.Value?.Url;
                    }

                     // MainImage
                    if (!string.IsNullOrEmpty(page.Logo?.Asset?.Value?.Url)) 
                    {
                        var code = $"{prefix}.logo";
                        result[lang][code] = page.Logo?.Asset?.Value?.Url;
                    }
                }
            }

            return result;

        }
    }
}
