using System.ComponentModel.DataAnnotations;
using TinaKuafor.Services;

namespace TinaKuafor.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Notes { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        public DateTime CreatedAt { get; set; } = TimeService.Now;

        public AppointmentStatus Status { get; set; } = AppointmentStatus.Confirmed;

        // Telegram bildirim gönderildi mi?
        public bool ReminderSent { get; set; } = false;

        // Takvim hatırlatması oluşturuldu mu?
        public bool CalendarReminderCreated { get; set; } = false;

        // Navigation property for services in this appointment
        public ICollection<AppointmentService> AppointmentServices { get; set; } = new List<AppointmentService>();

        // Total duration in minutes
        public int GetTotalDuration()
        {
            return (int)(EndTime - StartTime).TotalMinutes;
        }

        // Total price
        public decimal GetTotalPrice()
        {
            decimal total = 0;
            foreach (var appointmentService in AppointmentServices)
            {
                if (appointmentService.Service != null)
                {
                    total += appointmentService.Service.Price;
                }
            }
            return total;
        }

        // Randevu 10 dakika öncesi mi kontrol et
        public bool IsReminder10MinutesDue()
        {
            var appointmentDateTime = AppointmentDate.Date.Add(StartTime);
            var reminderTime = appointmentDateTime.AddMinutes(-10);
            var now = TimeService.Now;
            
            return now >= reminderTime && now < appointmentDateTime && !ReminderSent;
        }
    }

    public enum AppointmentStatus
    {
        Pending,
        Confirmed,
        Completed,
        Cancelled
    }
}
