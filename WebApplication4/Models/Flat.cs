using System.ComponentModel.DataAnnotations;

namespace WebApplication4.Models
{
    public class Flat
    {
        public int Id { get; set; }

        [Required]
        public string Address { get; set; }

        public int Price { get; set; }

        public int Rooms { get; set; }
    }
}
