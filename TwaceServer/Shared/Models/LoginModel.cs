using System.ComponentModel.DataAnnotations;

namespace TwaceServer.Shared.Models
{
    public class LoginModel
    {
        [Required]
        public string Login { get; set; }

        [Required]
        public string Password { get; set; }

        /// <summary>
        /// ID для Onesignal
        /// </summary>
        public string DeviceId { get; set; }
    }
}
