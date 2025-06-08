using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TinaKuafor.Data;
using TinaKuafor.Services;

public class DailyStatsService : IHostedService, IDisposable
{
    private readonly ILogger<DailyStatsService> _logger;
    private Timer _timer;
    private readonly IServiceScopeFactory _scopeFactory;

    public DailyStatsService(ILogger<DailyStatsService> logger, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Daily Stats Service running.");

        _timer = new Timer(DoWork, null, TimeSpan.Zero,
            TimeSpan.FromDays(1));

        return Task.CompletedTask;
    }

    private async void DoWork(object state)
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var telegramService = scope.ServiceProvider.GetRequiredService<ITelegramService>();

            var today = DateTime.Today;
            var appointments = await context.Appointments
                .Include(a => a.Service) // HATA DÜZELTİLDİ: Service bilgisi sorguya dahil edildi.
                .Where(a => a.DateTime.Date == today)
                .ToListAsync();

            var totalAppointments = appointments.Count;
            // HATA DÜZELTİLDİ: Null referans hatasını önlemek için null kontrolü eklendi.
            decimal totalRevenue = appointments.Sum(a => a.Service?.Price ?? 0);

            var message = $"Daily Stats for {today:yyyy-MM-dd}:\n" +
                          $"Total Appointments: {totalAppointments}\n" +
                          $"Total Revenue: {totalRevenue:C}";
            
            // HATA DÜZELTİLDİ: Mesajın gönderilmesi için await kullanıldı.
            await telegramService.SendMessageAsync(message);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Daily Stats Service is stopping.");

        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}