namespace Buk.Gaming.Models
{
    public class SanityResult<T>
    {
        public T Item { get; set; }
        
        public bool Success { get; set; }

        public string Reason { get; set; }
    }
}
