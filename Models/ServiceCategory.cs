using System.ComponentModel.DataAnnotations;

namespace TinaKuafor.Models
{
    public class ServiceCategory
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string UrlSlug { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(1000)]
        public string? SeoDescription { get; set; }

        [StringLength(200)]
        public string? SeoKeywords { get; set; }

        public int DisplayOrder { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        // Navigation property for services in this category
        public ICollection<Service> Services { get; set; } = new List<Service>();
    }
}