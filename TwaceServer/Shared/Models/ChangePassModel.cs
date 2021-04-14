using System.ComponentModel.DataAnnotations;

namespace TwaceServer.Shared.Models
{
    public class ChangePassModel
    {
        [Required]
        public string OldPass { get; set; }

        [Required]
        public string NewPass { get; set; }
    }
}
