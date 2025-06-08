using TinaKuafor.Models;

namespace TinaKuafor.Services
{
    public interface IAppointmentService
    {
        Task<List<Appointment>> GetAppointmentsAsync(DateTime date);
        Task<Appointment?> GetAppointmentByIdAsync(int id);
        Task<List<TimeSlot>> GetAvailableTimeSlotsAsync(DateTime date, List<int> serviceIds);
        Task<Appointment> CreateAppointmentAsync(Appointment appointment, List<int> serviceIds);
        Task<bool> UpdateAppointmentAsync(Appointment appointment);
        Task<bool> DeleteAppointmentAsync(int id);
        Task<bool> IsTimeSlotAvailableAsync(DateTime date, TimeSpan startTime, TimeSpan endTime, int? excludeAppointmentId = null);
        Task<List<BusinessHours>> GetBusinessHoursAsync();
        Task<bool> UpdateBusinessHoursAsync(BusinessHours businessHours);
        Task<bool> IsDayAvailableForBookingAsync(DateTime date);
    }

    public class TimeSlot
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsAvailable { get; set; }
    }
}