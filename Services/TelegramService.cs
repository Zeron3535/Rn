using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using TinaKuafor.Data;
using TinaKuafor.Models;

namespace TinaKuafor.Services
{
    public class TelegramService : ITelegramService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TelegramService> _logger;
        private readonly ApplicationDbContext _context;
        private TelegramBotClient? _botClient;

        public TelegramService(IConfiguration configuration, ILogger<TelegramService> logger, ApplicationDbContext context)
        {
            _configuration = configuration;
            _logger = logger;
            _context = context;
        }

        private async Task<TelegramBotClient> GetBotClientAsync()
        {
            if (_botClient != null)
                return _botClient;

            // Try to get from database first
            var settings = await _context.SiteSettings.FirstOrDefaultAsync();
            string? apiKey = settings?.TelegramApiKey;

            // If not in database, get from configuration
            if (string.IsNullOrEmpty(apiKey))
            {
                apiKey = _configuration["TelegramBot:ApiKey"];
                _logger.LogWarning("Telegram API key not found in database, using configuration value");
            }

            if (string.IsNullOrEmpty(apiKey))
            {
                _logger.LogError("Telegram API key is not configured in database or appsettings.json");
                throw new InvalidOperationException("Telegram API key is not configured");
            }

            try
            {
                _logger.LogInformation($"Creating Telegram bot client with API key: {apiKey[..6]}...");
                _botClient = new TelegramBotClient(apiKey);

                // Verify the API key is valid by making a test call
                var me = await _botClient.GetMeAsync();
                _logger.LogInformation($"Successfully connected to Telegram bot: @{me.Username}");

                return _botClient;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Telegram bot client. Invalid API key?");
                throw new InvalidOperationException("Failed to create Telegram bot client: " + ex.Message, ex);
            }
        }

        private async Task<List<string>> GetChatIdsAsync()
        {
            try {
                // Try to get from database first
                var settings = await _context.SiteSettings.FirstOrDefaultAsync();

                if (settings != null && !string.IsNullOrEmpty(settings.TelegramChatId))
                {
                    var chatIds = settings.GetTelegramChatIds();
                    _logger.LogInformation($"Using {chatIds.Count} chat IDs from database: {string.Join(", ", chatIds)}");
                    return chatIds;
                }

                // If not in database, get from configuration
                string? configChatId = _configuration["TelegramBot:ChatId"];

                if (!string.IsNullOrEmpty(configChatId))
                {
                    var chatIds = configChatId.Split(new[] {',', ';'}, StringSplitOptions.RemoveEmptyEntries)
                        .Select(id => id.Trim())
                        .Where(id => !string.IsNullOrEmpty(id))
                        .ToList();

                    _logger.LogInformation($"Using {chatIds.Count} chat IDs from configuration: {string.Join(", ", chatIds)}");
                    return chatIds;
                }

                _logger.LogError("Telegram chat ID is not configured in database or appsettings.json");
                throw new InvalidOperationException("Telegram chat ID is not configured");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving chat IDs");
                throw;
            }
        }

        public async Task SendAppointmentNotificationAsync(Appointment appointment)
        {
            try
            {
                _logger.LogInformation($"Preparing to send appointment notification for {appointment.CustomerName} on {appointment.AppointmentDate.ToShortDateString()}");

                var botClient = await GetBotClientAsync();

                // Get all configured chat IDs
                var chatIds = await GetChatIdsAsync();

                if (chatIds.Count == 0)
                {
                    _logger.LogWarning("No chat IDs available for notification");
                    return;
                }

                // Get appointment number for today
                var todayAppointments = await _context.Appointments
                    .Where(a => a.AppointmentDate.Date == appointment.AppointmentDate.Date)
                    .OrderBy(a => a.StartTime)
                    .ToListAsync();

                var appointmentNumber = todayAppointments.FindIndex(a => a.Id == appointment.Id) + 1;

                // Format the message
                var serviceNames = string.Join(", ", appointment.AppointmentServices
                    .Select(a => a.Service?.Name ?? "Bilinmeyen Hizmet"));

                var message = $"🔔 *Yeni Randevu Alındı!*\n\n" +
                              $"*{appointmentNumber}. Müşteri:* {appointment.CustomerName}\n" +
                              $"*Telefon:* {appointment.PhoneNumber}\n" +
                              $"*Tarih:* {appointment.AppointmentDate:dd.MM.yyyy}\n" +
                              $"*Saat:* {appointment.StartTime:hh\\:mm} - {appointment.EndTime:hh\\:mm}\n" +
                              $"*Hizmetler:* {serviceNames}\n" +
                              $"*Toplam Süre:* {appointment.GetTotalDuration()} dakika\n" +
                              $"*Toplam Ücret:* {appointment.GetTotalPrice():C2}\n";

                if (!string.IsNullOrEmpty(appointment.Notes))
                {
                    message += $"*Not:* {appointment.Notes}\n";
                }

                _logger.LogInformation($"Sending notification to {chatIds.Count} chat IDs: {string.Join(", ", chatIds)}");

                // Send to all chat IDs
                foreach (var chatId in chatIds)
                {
                    try
                    {
                        _logger.LogInformation($"Sending appointment notification to Telegram chat ID: {chatId}");

                        // Handle both numeric IDs and usernames/channels
                        ChatId telegramChatId;
                        if (long.TryParse(chatId, out var numericId))
                        {
                            telegramChatId = new ChatId(numericId);
                        }
                        else
                        {
                            telegramChatId = new ChatId(chatId);
                        }

                        await botClient.SendTextMessageAsync(
                            chatId: telegramChatId,
                            text: message,
                            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);

                        _logger.LogInformation($"Appointment notification sent to Telegram chat ID: {chatId}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error sending appointment notification to Telegram chat ID: {chatId}. Error: {ex.Message}");
                        // Continue with other chat IDs even if one fails
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending appointment notification to Telegram: {ex.Message}");
                // Don't throw the exception to avoid breaking the appointment process
            }
        }

        public async Task SendAppointmentReminderAsync(Appointment appointment)
        {
            try
            {
                _logger.LogInformation($"Preparing to send appointment reminder for {appointment.CustomerName}");

                var botClient = await GetBotClientAsync();

                // Get all configured chat IDs
                var chatIds = await GetChatIdsAsync();

                if (chatIds.Count == 0)
                {
                    _logger.LogWarning("No chat IDs available for reminder");
                    return;
                }

                // Get appointment number for today
                var todayAppointments = await _context.Appointments
                    .Where(a => a.AppointmentDate.Date == appointment.AppointmentDate.Date)
                    .OrderBy(a => a.StartTime)
                    .ToListAsync();

                var appointmentNumber = todayAppointments.FindIndex(a => a.Id == appointment.Id) + 1;

                // Format the reminder message
                var serviceNames = string.Join(", ", appointment.AppointmentServices
                    .Select(a => a.Service?.Name ?? "Bilinmeyen Hizmet"));

                var message = $"⏰ *Randevu Hatırlatması - 10 Dakika Kaldı!*\n\n" +
                              $"*{appointmentNumber}. Müşteri:* {appointment.CustomerName}\n" +
                              $"*Telefon:* {appointment.PhoneNumber}\n" +
                              $"*Saat:* {appointment.StartTime:hh\\:mm} - {appointment.EndTime:hh\\:mm}\n" +
                              $"*Hizmetler:* {serviceNames}\n" +
                              $"*Süre:* {appointment.GetTotalDuration()} dakika\n\n" +
                              $"🔔 Randevusuna 10 dakika kaldı!";

                _logger.LogInformation($"Sending reminder to {chatIds.Count} chat IDs");

                // Send to all chat IDs
                foreach (var chatId in chatIds)
                {
                    try
                    {
                        // Handle both numeric IDs and usernames/channels
                        ChatId telegramChatId;
                        if (long.TryParse(chatId, out var numericId))
                        {
                            telegramChatId = new ChatId(numericId);
                        }
                        else
                        {
                            telegramChatId = new ChatId(chatId);
                        }

                        await botClient.SendTextMessageAsync(
                            chatId: telegramChatId,
                            text: message,
                            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);

                        _logger.LogInformation($"Appointment reminder sent to Telegram chat ID: {chatId}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error sending appointment reminder to Telegram chat ID: {chatId}. Error: {ex.Message}");
                    }
                }

                // Mark reminder as sent
                appointment.ReminderSent = true;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending appointment reminder to Telegram: {ex.Message}");
            }
        }

        // Validate chat ID format
        private bool IsValidChatId(string chatId)
        {
            // Clean up the chatId
            chatId = chatId.Trim();

            // Handle numeric chat IDs (user IDs or group IDs)
            if (long.TryParse(chatId, out _))
                return true;

            // Handle usernames or channel names with @ format
            if (chatId.StartsWith("@") && chatId.Length > 1)
                return true;

            // For username format without @, add flexibility
            if (!chatId.StartsWith("@") && !chatId.Contains(" ") && chatId.Length > 3)
                return true;

            return false;
        }

        public async Task SendHelpMessageAsync(string chatId)
        {
            try
            {
                var botClient = await GetBotClientAsync();

                var helpMessage = $"🤖 *Tina Kuaför Bot Yardım*\n\n" +
                                 $"*Kullanılabilir Komutlar:*\n\n" +
                                 $"📅 `/bugun` - Bugünkü randevuları göster\n" +
                                 $"📊 `/istatistik` - Günlük istatistikleri göster\n" +
                                 $"💰 `/gelir` - Bu ay gelir özeti\n" +
                                 $"📋 `/belgeler` - Bekleyen belgeler\n" +
                                 $"⏰ `/hatirlatma` - Yaklaşan randevular\n" +
                                 $"🔧 `/ayarlar` - Bot ayarları\n" +
                                 $"❓ `/yardim` - Bu yardım mesajı\n\n" +
                                 $"*Otomatik Bildirimler:*\n" +
                                 $"• Yeni randevu alındığında\n" +
                                 $"• Randevudan 10 dk önce hatırlatma\n" +
                                 $"• Günlük özet (akşam 20:00)\n\n" +
                                 $"💡 *İpucu:* Komutları / ile başlatarak kullanabilirsiniz.";

                ChatId telegramChatId;
                if (long.TryParse(chatId, out var numericId))
                {
                    telegramChatId = new ChatId(numericId);
                }
                else
                {
                    telegramChatId = new ChatId(chatId);
                }

                await botClient.SendTextMessageAsync(
                    chatId: telegramChatId,
                    text: helpMessage,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);

                _logger.LogInformation($"Help message sent to Telegram chat ID: {chatId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending help message to Telegram chat ID: {chatId}");
            }
        }

        public async Task SendDailyStatsAsync()
        {
            try
            {
                var chatIds = await GetChatIdsAsync();
                if (chatIds.Count == 0) return;

                var today = DateTime.Today;
                var todayAppointments = await _context.Appointments
                    .Include(a => a.AppointmentServices)
                        .ThenInclude(a => a.Service)
                    .Where(a => a.AppointmentDate.Date == today)
                    .ToListAsync();

                var completedAppointments = todayAppointments.Where(a => a.Status == AppointmentStatus.Completed).ToList();
                var upcomingAppointments = todayAppointments.Where(a => a.Status == AppointmentStatus.Confirmed).ToList();

                var dailyIncome = completedAppointments.Sum(a => a.GetTotalPrice());

                var message = $"📊 *Günlük Özet - {today:dd.MM.yyyy}*\n\n" +
                             $"📅 *Randevular:*\n" +
                             $"• Toplam: {todayAppointments.Count}\n" +
                             $"• Tamamlanan: {completedAppointments.Count}\n" +
                             $"• Bekleyen: {upcomingAppointments.Count}\n\n" +
                             $"💰 *Gelir:*\n" +
                             $"• Bugünkü gelir: ₺{dailyIncome:N0}\n\n" +
                             $"⏰ Yarın için {await GetTomorrowAppointmentCount()} randevu planlanmış.\n\n" +
                             $"✨ Güzel bir gün geçirdiniz!";

                foreach (var chatId in chatIds)
                {
                    try
                    {
                        ChatId telegramChatId;
                        if (long.TryParse(chatId, out var numericId))
                        {
                            telegramChatId = new ChatId(numericId);
                        }
                        else
                        {
                            telegramChatId = new ChatId(chatId);
                        }

                        await (await GetBotClientAsync()).SendTextMessageAsync(
                            chatId: telegramChatId,
                            text: message,
                            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error sending daily stats to chat ID: {chatId}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending daily stats");
            }
        }

        private async Task<int> GetTomorrowAppointmentCount()
        {
            var tomorrow = DateTime.Today.AddDays(1);
            return await _context.Appointments
                .Where(a => a.AppointmentDate.Date == tomorrow && a.Status == AppointmentStatus.Confirmed)
                .CountAsync();
        }

        public async Task SendTestMessageAsync(string message)
        {
            try
            {
                _logger.LogInformation("Starting Telegram test message");
                var botClient = await GetBotClientAsync();

                // Get all configured chat IDs
                var chatIds = await GetChatIdsAsync();

                if (chatIds.Count == 0)
                {
                    _logger.LogWarning("No chat IDs available for test message");
                    throw new InvalidOperationException("No Telegram chat IDs configured");
                }

                _logger.LogInformation($"Sending test message to {chatIds.Count} chat IDs: {string.Join(", ", chatIds)}");

                List<Exception> exceptions = new List<Exception>();

                // Send to all chat IDs
                foreach (var chatId in chatIds)
                {
                    try
                    {
                        _logger.LogInformation($"Sending test message to Telegram chat ID: {chatId}");

                        // Handle both numeric IDs and usernames/channels
                        ChatId telegramChatId;
                        if (long.TryParse(chatId, out var numericId))
                        {
                            telegramChatId = new ChatId(numericId);
                        }
                        else
                        {
                            telegramChatId = new ChatId(chatId);
                        }

                        await botClient.SendTextMessageAsync(
                            chatId: telegramChatId,
                            text: message);

                        _logger.LogInformation($"Test message sent to Telegram chat ID: {chatId}");
                    }
                    catch (Exception ex)
                    {
                        var errorMsg = $"Error sending to {chatId}: {ex.Message}";
                        _logger.LogError(ex, errorMsg);
                        exceptions.Add(ex);
                    }
                }

                // If all sends failed, throw an exception
                if (exceptions.Count == chatIds.Count)
                {
                    throw new AggregateException($"Failed to send test message to all {chatIds.Count} chat IDs", exceptions);
                }
                // If some sends failed, log a warning
                else if (exceptions.Count > 0)
                {
                    _logger.LogWarning($"Test message sent to {chatIds.Count - exceptions.Count} of {chatIds.Count} chat IDs");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending test message to Telegram: {ex.Message}");
                throw; // Rethrow for test messages to show the error
            }
        }
    }
}
