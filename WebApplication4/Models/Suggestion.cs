using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication4.Models
{
    public enum ObjectType
    {
        [Display(Name = "Квартира")]
        Flat,
        [Display(Name = "Апартаменты")]
        Apartments,
        [Display(Name = "Комната")]
        Room,
        [Display(Name = "Доля в квартире")]
        Share,
        [Display(Name = "Другое")]
        Other
    }

    public enum RoomsCount
    {
        [Display(Name = "Студия")]
        Studio,
        [Display(Name = "1")]
        One,
        [Display(Name = "2")]
        Two,
        [Display(Name = "3")]
        Three,
        [Display(Name = "4+")]
        FourPlus
    }

    public enum EncumbranceType
    {
        [Display(Name = "Нет")]
        None,
        [Display(Name = "Ипотека")]
        Mortgage,
        [Display(Name = "Коммунальные долги")]
        Utilities,
        [Display(Name = "Другое")]
        Other
    }

    public enum ConditionType
    {
        [Display(Name = "Стандартный ремонт")]
        Standard,
        [Display(Name = "Требуется ремонт")]
        NeedsRepair,
        [Display(Name = "Ремонт от застройщика")]
        Developer,
        [Display(Name = "Чистовая отделка")]
        Finished,
        [Display(Name = "Дизайнерский ремонт")]
        Designer
    }

    public class Suggestion
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Выберите тип объекта")]
        [Display(Name = "Что хотите продать?")]
        public ObjectType ObjectType { get; set; }

        [Required(ErrorMessage = "Укажите площадь")]
        [Range(1, 10000, ErrorMessage = "Площадь должна быть от 1 до 10000 м²")]
        [Display(Name = "Площадь (м²)")]
        public double Area { get; set; }

        [Required(ErrorMessage = "Укажите количество комнат")]
        [Display(Name = "Количество комнат")]
        public RoomsCount Rooms { get; set; }

        [Required(ErrorMessage = "Укажите наличие обременений")]
        [Display(Name = "Обременения")]
        public EncumbranceType Encumbrance { get; set; }

        [Required(ErrorMessage = "Укажите состояние")]
        [Display(Name = "Состояние квартиры")]
        public ConditionType Condition { get; set; }

        [Required(ErrorMessage = "Введите адрес")]
        [StringLength(200)]
        [Display(Name = "Адрес")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите номер телефона")]
        [Phone(ErrorMessage = "Некорректный номер телефона")]
        [Display(Name = "Номер телефона для связи")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Range(0, double.MaxValue, ErrorMessage = "Цена не может быть отрицательной")]
        [Display(Name = "Цена (руб.)")]
        public decimal? Price { get; set; }
        public string? Description { get; set; }

        public string? UserId { get; set; }
        public SuggestionStatus Status { get; set; } = SuggestionStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ApprovedAt { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime? RejectedAt { get; set; }
        public string? RejectedBy { get; set; }
        public string? ImageUrl { get; set; }
    }

    public enum SuggestionStatus
    {
        Pending,
        Approved,
        Rejected
    }
}