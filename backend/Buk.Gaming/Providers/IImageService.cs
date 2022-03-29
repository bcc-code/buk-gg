using System.Threading.Tasks;


// Source: Oslofjord
namespace Buk.Gaming.Providers
{

    public class ImageTransform
    {
        public int? Height { get; set; }

        public int? Width { get; set; }

        public int? MinHeight { get; set; }

        public int? MaxHeight { get; set; }

        public int? MinWidth { get; set; }

        public int? MaxWidth { get; set; }

        public int? Blur { get; set; }

        public string BackgroundColor { get; set; }

        public ImageTransformMode Mode { get; set; }
    }

    public enum ImageTransformMode
    {
        Crop = 0,
        Pad = 1, // Sanity: clip
        Max = 2,
        Carve = 3,
        Stretch = 4 // Sanity: scale
    }

    public interface IImageService
    {
        Task<string> GetBase64ImageContentAsync(string originalUrl, ImageTransform transform = null);

        string GetBase64ImageUrl(string originalUrl, ImageTransform transform = null);

        string GetCachedImageUrl(string originalUrl, ImageTransform transform = null);

    }
}
