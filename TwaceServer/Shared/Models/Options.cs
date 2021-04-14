using System.ComponentModel.DataAnnotations;

namespace TwaceServer.Shared.Models
{
    public class Options
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
