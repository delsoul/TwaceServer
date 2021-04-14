using System.ComponentModel.DataAnnotations;

namespace TwaceServer.Shared.Models
{
    public class Dish
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        public string Kcal { get; set; }
        public int Count { get; set; }
    }
}
