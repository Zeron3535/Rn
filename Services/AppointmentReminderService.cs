using Microsoft.EntityFrameworkCore;
using TinaKuafor.Data;
using TinaKuafor.Models;

namespace TinaKuafor.Services
{
    public class AppointmentReminderService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AppointmentReminderService> _logger;

        public AppointmentReminderService(IServiceProvider serviceProvider, ILogger<AppointmentReminderService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckAndSendReminders();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in appointment reminder service");
                }

                // Check every minute
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private async Task CheckAndSendReminders()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var telegramService = scope.ServiceProvider.GetRequiredService<ITelegramService>();

            var now = TimeService.Now;
            var today = now.Date;

            // Get appointments that need reminders (10 minutes before start time)
            var appointments = await context.Appointments
                .Include(a => a.AppointmentServices)
                    .ThenInclude(a => a.Service)
                .Where(a => a.AppointmentDate.Date == today && 
                           a.Status == AppointmentStatus.Confirmed &&
                           !a.ReminderSent)
                .ToListAsync();

            foreach (var appointment in appointments)
            {
                if (appointment.IsReminder10MinutesDue())
                {
                    _logger.LogInformation($"Sending 10-minute reminder for appointment {appointment.Id} - {appointment.CustomerName}");
                    await telegramService.SendAppointmentReminderAsync(appointment);
                }
            }
        }
    }
} 