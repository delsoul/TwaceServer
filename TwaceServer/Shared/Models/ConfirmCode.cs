using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TwaceServer.Shared.Models
{
    public class ConfirmCode
    {
        [Key]
        [ForeignKey("User")]
        public string Id { get; set; }

        /// <summary>
        /// Код потдверждения 
        /// </summary>
        public string VerifyCode { get; set; }

        [JsonIgnore]
        public virtual ApplicationUser User { get; set; }
    }
}
