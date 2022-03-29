using System.Threading.Tasks;
using System.Collections.Generic;
namespace Buk.Gaming.Models
{
    public class DiscordUser
    {
        public string Id { get; set; }

        public string Tag { get; set; }

        public string DisplayName { get; set; }

        public string Username { get; set; }

        public bool Bot { get; set; }

        public string Discriminator { get; set; }

        public string Avatar { get; set; }

        public string Created { get; set; }
    }
}