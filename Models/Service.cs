using System.ComponentModel.DataAnnotations;

namespace TinaKuafor.Models
{
    public class Service
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string UrlSlug { get; set; } = string.Empty;

        [Required]
        [Range(0, 10000)]
        public decimal Price { get; set; }

        [Required]
        [Range(5, 240)]
        public int DurationMinutes { get; set; } = 30;

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(1000)]
        public string? SeoDescription { get; set; }

        [StringLength(200)]
        public string? SeoKeywords { get; set; }

        public bool IsActive { get; set; } = true;

        public int CategoryId { get; set; }
        public ServiceCategory? Category { get; set; }

        // Navigation property for appointments
        public ICollection<AppointmentService> AppointmentServices { get; set; } = new List<AppointmentService>();
    }
}