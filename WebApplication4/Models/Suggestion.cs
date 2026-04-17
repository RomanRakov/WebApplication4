using System.ComponentModel.DataAnnotations;
using WebApplication4.Models;

namespace WebApplication4.Models
{
    // Статусы для модерации
    public enum SuggestionStatus
    {
        Pending,   // На рассмотрении
        Approved,  // Одобрено
        Rejected   // Отклонено
    }

    public class Suggestion
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = null!;

        [Required]
        [Display(Name = "Тип недвижимости")]
        public string PropertyType { get; set; } = null!;

        [Required]
        [Range(10, 1000)]
        [Display(Name = "Площадь (м²)")]
        public decimal Area { get; set; }

        [Required]
        [Display(Name = "Количество комнат")]
        public int Rooms { get; set; }

        [Required]
        [Display(Name = "Цена")]
        public decimal Price { get; set; }

        [Required]
        [Display(Name = "Адрес")]
        [MaxLength(200)]
        public string Address { get; set; } = null!;

        [Display(Name = "Состояние")]
        public string Condition { get; set; } = null!;

        [Display(Name = "Описание")]
        [MaxLength(2000)]
        public string Description { get; set; } = null!;

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        [Required]
        public string UserId { get; set; } // Ссылка на IdentityUser

        [Display(Name = "Статус")]
        public SuggestionStatus Status { get; set; } = SuggestionStatus.Pending;

        [Display(Name = "Дата создания")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Для админа
        public DateTime? ApprovedAt { get; set; }
        public DateTime? RejectedAt { get; set; }
        public string? ApprovedBy { get; set; }
        public string? RejectedBy { get; set; }
    }
}
