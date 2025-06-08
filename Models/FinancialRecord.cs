using System.ComponentModel.DataAnnotations;

namespace TinaKuafor.Models
{
    public class FinancialRecord
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public FinancialRecordType Type { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [StringLength(50)]
        public string Category { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Randevu ile ilişkili mi? (Gelir için)
        public int? AppointmentId { get; set; }
        public Appointment? Appointment { get; set; }
    }

    public enum FinancialRecordType
    {
        Income,    // Gelir
        Expense    // Gider
    }
} 