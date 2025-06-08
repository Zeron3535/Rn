using System.ComponentModel.DataAnnotations;

namespace TinaKuafor.Models
{
    public class BusinessDocument
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        [StringLength(50)]
        public string Category { get; set; } = string.Empty;

        [StringLength(20)]
        public string Priority { get; set; } = "Normal"; // Normal, Yüksek, Düşük

        public bool IsCompleted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? DueDate { get; set; }

        public DateTime? CompletedAt { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }
} 