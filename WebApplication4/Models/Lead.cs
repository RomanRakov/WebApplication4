using System.ComponentModel.DataAnnotations;

namespace WebApplication4.Models
{
    public class Lead
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите ваше имя")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Введите номер телефона")]
        [RegularExpression(@"^(\+7|8|7)[\s\-]?\(?[489][0-9]{2}\)?[\s\-]?[0-9]{3}[\s\-]?[0-9]{2}[\s\-]?[0-9]{2}$",
            ErrorMessage = "Введите корректный номер телефона (например, +7 999 000-00-00)")]
        public string Phone { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}