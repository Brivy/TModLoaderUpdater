using System.ComponentModel.DataAnnotations;

namespace TModLoaderMaintainer.Infrastructure.Server.Communication.Configuration
{
    public class ServerConnectionSettings
    {
        [Required]
        public string Username { get; set; } = null!;

        [Required]
        public string Host { get; set; } = null!;

        [Required]
        public int Port { get; set; }

        [Required]
        public string PrivateKeyLocation { get; set; } = null!;
    }
}
