using System.ComponentModel.DataAnnotations;

namespace TinaKuafor.Models
{
    public class BusinessHours
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(20)]
        public string DayOfWeek { get; set; } = string.Empty;
        
        public bool IsOpen { get; set; } = true;
        
        [StringLength(5)]
        public string OpenTime { get; set; } = "10:00";
        
        [StringLength(5)]
        public string CloseTime { get; set; } = "21:00";
        
        // Helper method to check if a specific time is within business hours
        public bool IsTimeWithinBusinessHours(TimeSpan time)
        {
            if (!IsOpen)
                return false;
                
            if (string.IsNullOrEmpty(OpenTime) || string.IsNullOrEmpty(CloseTime))
                return false;
                
            var openTimeParts = OpenTime.Split(':');
            var closeTimeParts = CloseTime.Split(':');
            
            if (openTimeParts.Length != 2 || closeTimeParts.Length != 2)
                return false;
                
            if (!int.TryParse(openTimeParts[0], out int openHour) || 
                !int.TryParse(openTimeParts[1], out int openMinute) ||
                !int.TryParse(closeTimeParts[0], out int closeHour) ||
                !int.TryParse(closeTimeParts[1], out int closeMinute))
                return false;
                
            var openTimeSpan = new TimeSpan(openHour, openMinute, 0);
            var closeTimeSpan = new TimeSpan(closeHour, closeMinute, 0);
            
            return time >= openTimeSpan && time <= closeTimeSpan;
        }
    }
}