using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwaceServer.Shared.Models.Enums;

namespace TwaceServer.Shared.Models
{
    public class Request
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public string Comment { get; set; }

        [Required]
        public RequestStatus Status { get; set; }

        [Required]
        public DateTime RequestDateTime { get; set; }
        public string RejectReason { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        [JsonIgnore]
        public virtual ApplicationUser User { get; set; }

        [JsonIgnore]
        public virtual Dish Dishes { get; set; }

        [JsonIgnore]
        public virtual Options Options { get; set; }

    }
}
