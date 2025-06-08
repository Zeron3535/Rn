using TinaKuafor.Models;

namespace TinaKuafor.Services
{
    public interface ITelegramService
    {
        Task SendAppointmentNotificationAsync(Appointment appointment);
        Task SendAppointmentReminderAsync(Appointment appointment);
        Task SendTestMessageAsync(string message);
        Task SendHelpMessageAsync(string chatId);
        Task SendDailyStatsAsync();
    }
}