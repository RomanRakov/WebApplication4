using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication4.Models
{
    public class Flat
    {
        public int Id { get; set; }

        public ObjectType ObjectType { get; set; }
        public double Area { get; set; }
        public RoomsCount Rooms { get; set; }
        public EncumbranceType Encumbrance { get; set; }
        public ConditionType Condition { get; set; }
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public decimal? Price { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }

        public DateTime PublishedAt { get; set; }
        public bool IsPublished { get; set; }
        public string? ModeratorUserId { get; set; }
    }
}