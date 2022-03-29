using Buk.Gaming.Providers;
using Newtonsoft.Json.Linq;
using Sanity.Linq;
using Sanity.Linq.CommonTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Buk.Gaming.Sanity.Serializers
{
    public class SanityImageUtilities
    {
        public static Func<JToken, SanityOptions, Task<string>> CreateCachedSerializer(IImageService imageSvc)
        {
            return (JToken input, SanityOptions options) =>
            {
                var asset = input["asset"];
                var imageRef = asset?["_ref"]?.ToString();

                if (asset == null || imageRef == null)
                {
                    return Task.FromResult("");
                }

                var parameters = new StringBuilder();

                if (input["query"] != null)
                {
                    parameters.Append($"?{(string)input["query"]}");
                }

                //build url
                var imageParts = imageRef.Split('-');
                var url = new StringBuilder();
                url.Append("https://cdn.sanity.io/");
                url.Append(imageParts[0] + "s/");            // images/
                url.Append(options.ProjectId + "/");             // projectid/
                url.Append(options.Dataset + "/");             // dataset/
                url.Append(imageParts[1] + "-");             // asset id-
                url.Append(imageParts[2] + ".");             // dimensions.
                url.Append(imageParts[3]);                       // file extension
                url.Append(parameters.ToString());                          // ?crop etc..

                var cachedUrl = imageSvc.GetCachedImageUrl(url.ToString());

                return Task.FromResult($"<figure><img src=\"{cachedUrl}\"/></figure>");
            };
        }

        public static string GetImageURL(SanityImage source, SanityOptions options)
        {
            if (source == null || source.Asset == null)
            {
                return null;
            }

            //build url
            var imageParts = source.Asset.Ref.Split('-');
            var url = new StringBuilder();
            url.Append("https://cdn.sanity.io/");
            url.Append(imageParts[0] + "s/");            // images/
            url.Append(options.ProjectId + "/");           // projectid/
            url.Append(options.Dataset + "/");             // dataset/
            url.Append(imageParts[1] + "-");             // asset id-
            url.Append(imageParts[2] + ".");             // dimensions.
            url.Append(imageParts[3]);                   // file extension

            return url.ToString();
        }

    }
}
