namespace Buk.Gaming.Models
{
    public class User : Player
    {
        public bool CanImpersonate { get; set; }

        public bool IsAdministrator { get; set; }

    }
}
