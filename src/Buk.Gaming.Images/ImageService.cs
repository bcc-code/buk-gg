using Buk.Gaming.Providers;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Buk.Gaming.Images
{

    public class ImageOptions
    {
        public string CacheServerPath { get; set; }
    }

    public class ImageService : IImageService
    {
        private static HttpClient _Client = new HttpClient();

        public ImageService(ImageOptions options, IMemoryCache cache)
        {
            Options = options;
            Cache = cache;
        }

        protected ImageOptions Options { get; }
        public IMemoryCache Cache { get; }

        // data:[<media type>][;charset=<character set>][;base64],<data>
        //  data:image/jpeg;charset=utf-8;base64,

        public async Task<string> GetBase64ImageContentAsync(string originalUrl, ImageTransform transform = null)
        {
            var url = GetCachedImageUrl(originalUrl, transform);
            if (!String.IsNullOrEmpty(url))
            {
                return await Cache.GetOrCreate("IMAGE_BASE64_" + url, async (c) =>
                {
                    var response = await _Client.GetAsync(url).ConfigureAwait(false);
                    if (response.IsSuccessStatusCode)
                    {
                        byte[] imageArray = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                        if (imageArray.Length > 0)
                        {
                            string base64ImageRepresentation = Convert.ToBase64String(imageArray);
                            var medaType = "image/jpeg";
                            if (originalUrl.Contains(".png"))
                            {
                                medaType = "image/png";
                            }
                            else if (originalUrl.Contains(".gif"))
                            {
                                medaType = "image/gif";
                            }

                            // Cache in memory for up to a month
                            c.AbsoluteExpiration = DateTimeOffset.Now.AddDays(30);
                            return $"data:{medaType};charset=utf-8;base64,{base64ImageRepresentation}";
                        }
                    }

                    // Don't cache failed responses
                    c.AbsoluteExpiration = DateTimeOffset.Now;
                    return "";
                });
            }
            return "";
        }

        public string GetCachedImageUrl(string originalUrl, ImageTransform transform = null)
        {
            if (!string.IsNullOrEmpty(originalUrl))
            {
                var sanityQueryParams = new List<string>();
                var cacheQueryParams = new List<string>();

                if (transform != null)
                {
                    if (transform.Width.HasValue && transform.Width.Value > 0)
                    {
                        sanityQueryParams.Add($"w={transform.Width.Value}");
                        cacheQueryParams.Add($"width={transform.Width.Value}");
                    }
                    if (transform.Height.HasValue && transform.Height.Value > 0)
                    {
                        sanityQueryParams.Add($"h={transform.Height.Value}");
                        cacheQueryParams.Add($"height={transform.Height.Value}");
                    }
                    if (transform.MaxWidth.HasValue && transform.MaxWidth.Value > 0)
                    {
                        sanityQueryParams.Add($"max-w={transform.MaxWidth.Value}");
                        cacheQueryParams.Add($"maxwidth={transform.MaxWidth.Value}");
                    }
                    if (transform.MaxHeight.HasValue && transform.MaxHeight.Value > 0)
                    {
                        sanityQueryParams.Add($"max-h={transform.MaxHeight.Value}");
                        cacheQueryParams.Add($"maxheight={transform.MaxHeight.Value}");
                    }
                    if (transform.MinWidth.HasValue && transform.MinWidth.Value > 0)
                    {
                        sanityQueryParams.Add($"min-w={transform.MinWidth.Value}");
                        cacheQueryParams.Add($"minwidth={transform.MinWidth.Value}");
                    }
                    if (transform.MinHeight.HasValue && transform.MinHeight.Value > 0)
                    {
                        sanityQueryParams.Add($"min-h={transform.MinHeight.Value}");
                        cacheQueryParams.Add($"minheight={transform.MinHeight.Value}");
                    }
                    if (transform.Blur.HasValue)
                    {
                        sanityQueryParams.Add($"blur={transform.Blur.Value}");
                    }
                    if (!string.IsNullOrEmpty(transform.BackgroundColor))
                    {
                        sanityQueryParams.Add($"bg={Uri.EscapeDataString(transform.BackgroundColor.TrimStart('#'))}");
                        cacheQueryParams.Add($"bgcolor={Uri.EscapeDataString(transform.BackgroundColor.TrimStart('#'))}");
                    }
                    switch (transform.Mode)
                    {
                        case ImageTransformMode.Crop:
                            {
                                sanityQueryParams.Add($"fit=crop");
                                cacheQueryParams.Add($"mode=crop");
                                break;
                            }
                        case ImageTransformMode.Pad: //clip
                            {
                                if (!string.IsNullOrEmpty(transform.BackgroundColor))
                                {
                                    sanityQueryParams.Add($"fit=clip");
                                }
                                else
                                {
                                    sanityQueryParams.Add($"fit=fill");
                                }
                                cacheQueryParams.Add($"mode=pad");
                                break;
                            }
                        case ImageTransformMode.Max:
                            {
                                sanityQueryParams.Add($"fit=fillmax");
                                cacheQueryParams.Add($"mode=max");
                                break;
                            }
                        case ImageTransformMode.Carve:
                            {
                                sanityQueryParams.Add($"crop=entropy"); //Sort of...
                                cacheQueryParams.Add($"mode=carve");
                                break;
                            }
                        case ImageTransformMode.Stretch:
                            {
                                sanityQueryParams.Add($"fit=scale");
                                cacheQueryParams.Add($"mode=stretch");
                                break;
                            }
                    }
                }

                var isSanityImage = originalUrl.IndexOf("sanity.io") != -1;
                var sourceUrl = originalUrl;
                if (isSanityImage)
                {
                    // Pass parameters to sanity
                    if (sanityQueryParams.Count > 0)
                    {
                        sourceUrl += (sourceUrl.Contains("?") ? "&" : "?") + sanityQueryParams.Aggregate((c, n) => c + "&" + n);
                    }
                }

                var cacheUrl = Options.CacheServerPath + "?image=" + Uri.EscapeDataString(sourceUrl);
                if ((!isSanityImage || (transform != null && transform.Mode == ImageTransformMode.Carve)) && cacheQueryParams.Count > 0)
                {
                    cacheUrl += "&" + cacheQueryParams.Aggregate((c, n) => c + "&" + n);
                }

                return cacheUrl;
            }
            return originalUrl;
        }

        public string GetBase64ImageUrl(string originalUrl, ImageTransform transform = null)
        {
            var url = GetCachedImageUrl(originalUrl, transform);
            if (url.Contains("?"))
            {
                return $"{url}&encoding=base64";
            }
            else
            {
                return $"{url}?encoding=base64";
            }
        }

    }
}
