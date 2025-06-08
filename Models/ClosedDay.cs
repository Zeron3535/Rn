using System.ComponentModel.DataAnnotations;

namespace TinaKuafor.Models
{
    public class ClosedDay
    {
        public int Id { get; set; }
        
        [Required]
        public DateTime Date { get; set; }
        
        [StringLength(255)]
        public string? Reason { get; set; }
    }
}