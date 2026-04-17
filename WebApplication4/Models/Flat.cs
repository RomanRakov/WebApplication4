using System.ComponentModel.DataAnnotations;

namespace WebApplication4.Models
{
    public class Flat
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

        [Required]
        [Display(Name = "Фотография")]
        [MaxLength(500)]
        public string ImageUrl { get; set; } = null!; 

        [Display(Name = "Дата публикации")]
        public DateTime PublishedAt { get; set; } = DateTime.UtcNow; 

        [Display(Name = "Статус")]
        public bool IsPublished { get; set; } = false; 

        [Display(Name = "Администратор")]
        public string? ModeratorUserId { get; set; } 
    }
}