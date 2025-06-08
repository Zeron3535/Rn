using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TinaKuafor.Data;
using TinaKuafor.Models;
using TinaKuafor.Services;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TinaKuafor.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAppointmentService _appointmentService;
        private readonly ITelegramService _telegramService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            ApplicationDbContext context,
            IAppointmentService appointmentService,
            ITelegramService telegramService,
            IConfiguration configuration,
            ILogger<AdminController> logger)
        {
            _context = context;
            _appointmentService = appointmentService;
            _telegramService = telegramService;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Get password from database or configuration
            var settings = await _context.SiteSettings.FirstOrDefaultAsync();
            string? correctPassword = settings?.AdminPassword;

            // If not in database, get from configuration
            if (string.IsNullOrEmpty(correctPassword))
                correctPassword = _configuration["AdminSettings:Password"];

            if (string.IsNullOrEmpty(correctPassword) || model.Password != correctPassword)
            {
                ModelState.AddModelError("", "Geçersiz şifre.");
                return View(model);
            }

            // Set admin session
            HttpContext.Session.SetString("IsAdmin", "true");

            return RedirectToAction(nameof(Dashboard));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            // Clear admin session
            HttpContext.Session.Remove("IsAdmin");

            return RedirectToAction("Index", "Home");
        }

        [AdminAuth]
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                _logger.LogInformation("Loading dashboard data");

                var today = DateTime.Today;
                var startOfMonth = new DateTime(today.Year, today.Month, 1);
                var endOfMonth = startOfMonth.AddMonths(1);

                // First get appointments without ordering by TimeSpan
                var appointments = await _context.Appointments
                    .Include(a => a.AppointmentServices)
                        .ThenInclude(a => a.Service)
                            .ThenInclude(s => s!.Category)
                    .Where(a => a.AppointmentDate.Date >= today)
                    .OrderBy(a => a.AppointmentDate)
                    .ToListAsync();

                // Then order by StartTime in memory to avoid SQLite limitation with TimeSpan
                appointments = appointments
                    .OrderBy(a => a.AppointmentDate)
                    .ThenBy(a => a.StartTime)
                    .Take(10)
                    .ToList();

                _logger.LogInformation($"Loaded {appointments.Count} upcoming appointments");

                var services = await _context.Services
                    .Where(s => s.IsActive)
                    .CountAsync();

                var testimonials = await _context.Testimonials
                    .Where(t => !t.IsApproved)
                    .CountAsync();

                // Finansal istatistikler
                var monthlyIncome = await _context.FinancialRecords
                    .Where(f => f.Type == FinancialRecordType.Income && 
                               f.Date >= startOfMonth && f.Date < endOfMonth)
                    .SumAsync(f => f.Amount);

                var monthlyExpense = await _context.FinancialRecords
                    .Where(f => f.Type == FinancialRecordType.Expense && 
                               f.Date >= startOfMonth && f.Date < endOfMonth)
                    .SumAsync(f => f.Amount);

                // Randevu geliri (bu ay tamamlanan randevular)
                var completedAppointments = await _context.Appointments
                    .Include(a => a.AppointmentServices)
                        .ThenInclude(a => a.Service)
                    .Where(a => a.Status == AppointmentStatus.Completed &&
                               a.AppointmentDate >= startOfMonth && a.AppointmentDate < endOfMonth)
                    .ToListAsync();

                var appointmentIncome = completedAppointments.Sum(a => a.GetTotalPrice());

                // Bekleyen belgeler
                var pendingDocuments = await _context.Documents
                    .Where(d => !d.IsCompleted)
                    .CountAsync();

                // Yüksek öncelikli belgeler
                var highPriorityDocuments = await _context.Documents
                    .Where(d => !d.IsCompleted && d.Priority == "Yüksek")
                    .CountAsync();

                var viewModel = new DashboardViewModel
                {
                    UpcomingAppointments = appointments,
                    ServiceCount = services,
                    PendingTestimonialCount = testimonials,
                    MonthlyIncome = monthlyIncome + appointmentIncome,
                    MonthlyExpense = monthlyExpense,
                    MonthlyProfit = (monthlyIncome + appointmentIncome) - monthlyExpense,
                    AppointmentIncome = appointmentIncome,
                    PendingDocuments = pendingDocuments,
                    HighPriorityDocuments = highPriorityDocuments,
                    CurrentMonth = today.ToString("MMMM yyyy")
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                // Log the error
                _logger.LogError(ex, "Error loading dashboard");

                // Return a simple view model to prevent errors
                var emptyViewModel = new DashboardViewModel
                {
                    UpcomingAppointments = new List<Appointment>(),
                    ServiceCount = 0,
                    PendingTestimonialCount = 0,
                    MonthlyIncome = 0,
                    MonthlyExpense = 0,
                    MonthlyProfit = 0,
                    AppointmentIncome = 0,
                    PendingDocuments = 0,
                    HighPriorityDocuments = 0,
                    CurrentMonth = DateTime.Today.ToString("MMMM yyyy")
                };

                // Add error message to TempData
                TempData["ErrorMessage"] = "Yönetim paneli yüklenirken bir hata oluştu. Lütfen tekrar deneyin.";

                return View(emptyViewModel);
            }
        }

        #region Services

        [AdminAuth]
        public async Task<IActionResult> Services()
        {
            try
            {
                _logger.LogInformation("Loading services data");

                var services = await _context.Services
                    .Include(s => s.Category)
                    .OrderBy(s => s.Category != null ? s.Category.DisplayOrder : 0)
                    .ThenBy(s => s.Name)
                    .ToListAsync();

                _logger.LogInformation($"Loaded {services.Count} services");

                return View(services);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading services");
                TempData["ErrorMessage"] = "Hizmetler yüklenirken bir hata oluştu. Lütfen tekrar deneyin.";
                return View(new List<Service>());
            }
        }

        [AdminAuth]
        public async Task<IActionResult> ServiceCreate()
        {
            var categories = await _context.ServiceCategories
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync();

            var viewModel = new ServiceEditViewModel
            {
                Service = new Service(),
                Categories = categories
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuth]
        public async Task<IActionResult> ServiceCreate(ServiceEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await _context.ServiceCategories
                    .OrderBy(c => c.DisplayOrder)
                    .ToListAsync();

                return View(model);
            }

            // Generate URL slug if not provided
            if (string.IsNullOrEmpty(model.Service.UrlSlug))
            {
                model.Service.UrlSlug = GenerateSlug(model.Service.Name);
            }

            // Check if slug already exists
            var existingService = await _context.Services
                .FirstOrDefaultAsync(s => s.UrlSlug == model.Service.UrlSlug);

            if (existingService != null)
            {
                ModelState.AddModelError("Service.UrlSlug", "Bu URL slug zaten kullanılıyor.");

                model.Categories = await _context.ServiceCategories
                    .OrderBy(c => c.DisplayOrder)
                    .ToListAsync();

                return View(model);
            }

            _context.Services.Add(model.Service);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Hizmet başarıyla oluşturuldu.";
            return RedirectToAction(nameof(Services));
        }

        [AdminAuth]
        public async Task<IActionResult> ServiceEdit(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null)
            {
                return NotFound();
            }

            var categories = await _context.ServiceCategories
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync();

            var viewModel = new ServiceEditViewModel
            {
                Service = service,
                Categories = categories
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuth]
        public async Task<IActionResult> ServiceEdit(ServiceEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await _context.ServiceCategories
                    .OrderBy(c => c.DisplayOrder)
                    .ToListAsync();

                return View(model);
            }

            // Generate URL slug if not provided
            if (string.IsNullOrEmpty(model.Service.UrlSlug))
            {
                model.Service.UrlSlug = GenerateSlug(model.Service.Name);
            }

            // Check if slug already exists (excluding this service)
            var existingService = await _context.Services
                .FirstOrDefaultAsync(s => s.UrlSlug == model.Service.UrlSlug && s.Id != model.Service.Id);

            if (existingService != null)
            {
                ModelState.AddModelError("Service.UrlSlug", "Bu URL slug zaten kullanılıyor.");

                model.Categories = await _context.ServiceCategories
                    .OrderBy(c => c.DisplayOrder)
                    .ToListAsync();

                return View(model);
            }

            _context.Update(model.Service);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Hizmet başarıyla güncellendi.";
            return RedirectToAction(nameof(Services));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuth]
        public async Task<IActionResult> ServiceDelete(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null)
            {
                return NotFound();
            }

            _context.Services.Remove(service);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Hizmet başarıyla silindi.";
            return RedirectToAction(nameof(Services));
        }

        #endregion

        #region Categories

        [AdminAuth]
        public async Task<IActionResult> Categories()
        {
            var categories = await _context.ServiceCategories
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync();

            return View(categories);
        }

        [AdminAuth]
        public IActionResult CategoryCreate()
        {
            return View(new ServiceCategory());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuth]
        public async Task<IActionResult> CategoryCreate(ServiceCategory category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }

            // Generate URL slug if not provided
            if (string.IsNullOrEmpty(category.UrlSlug))
            {
                category.UrlSlug = GenerateSlug(category.Name);
            }

            // Check if slug already exists
            var existingCategory = await _context.ServiceCategories
                .FirstOrDefaultAsync(c => c.UrlSlug == category.UrlSlug);

            if (existingCategory != null)
            {
                ModelState.AddModelError("UrlSlug", "Bu URL slug zaten kullanılıyor.");
                return View(category);
            }

            _context.ServiceCategories.Add(category);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Kategori başarıyla oluşturuldu.";
            return RedirectToAction(nameof(Categories));
        }

        [AdminAuth]
        public async Task<IActionResult> CategoryEdit(int id)
        {
            var category = await _context.ServiceCategories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuth]
        public async Task<IActionResult> CategoryEdit(ServiceCategory category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }

            // Generate URL slug if not provided
            if (string.IsNullOrEmpty(category.UrlSlug))
            {
                category.UrlSlug = GenerateSlug(category.Name);
            }

            // Check if slug already exists (excluding this category)
            var existingCategory = await _context.ServiceCategories
                .FirstOrDefaultAsync(c => c.UrlSlug == category.UrlSlug && c.Id != category.Id);

            if (existingCategory != null)
            {
                ModelState.AddModelError("UrlSlug", "Bu URL slug zaten kullanılıyor.");
                return View(category);
            }

            _context.Update(category);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Kategori başarıyla güncellendi.";
            return RedirectToAction(nameof(Categories));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuth]
        public async Task<IActionResult> CategoryDelete(int id)
        {
            var category = await _context.ServiceCategories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            // Check if category has services
            var hasServices = await _context.Services.AnyAsync(s => s.CategoryId == id);
            if (hasServices)
            {
                TempData["ErrorMessage"] = "Bu kategoriye ait hizmetler bulunduğu için silinemez.";
                return RedirectToAction(nameof(Categories));
            }

            _context.ServiceCategories.Remove(category);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Kategori başarıyla silindi.";
            return RedirectToAction(nameof(Categories));
        }

        #endregion

        #region Appointments

        [AdminAuth]
        public async Task<IActionResult> Appointments(DateTime? date)
        {
            try
            {
                date ??= DateTime.Today;
                _logger.LogInformation($"Loading appointments for date: {date.Value.ToShortDateString()}");

                var appointments = await _appointmentService.GetAppointmentsAsync(date.Value);
                _logger.LogInformation($"Loaded {appointments.Count} appointments for {date.Value.ToShortDateString()}");

                var viewModel = new AppointmentsViewModel
                {
                    Date = date.Value,
                    Appointments = appointments
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading appointments for date: {date?.ToShortDateString() ?? "null"}");
                TempData["ErrorMessage"] = "Randevular yüklenirken bir hata oluştu. Lütfen tekrar deneyin.";

                return View(new AppointmentsViewModel
                {
                    Date = date ?? DateTime.Today,
                    Appointments = new List<Appointment>()
                });
            }
        }

        [AdminAuth]
        public async Task<IActionResult> AppointmentDetails(int id)
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuth]
        public async Task<IActionResult> AppointmentDelete(int id)
        {
            await _appointmentService.DeleteAppointmentAsync(id);

            TempData["SuccessMessage"] = "Randevu başarıyla silindi.";
            return RedirectToAction(nameof(Appointments));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuth]
        public async Task<IActionResult> UpdateAppointmentStatus(int id, AppointmentStatus status)
        {
            try
            {
                _logger.LogInformation($"Updating appointment status for ID: {id} to status: {status}");

                var appointment = await _context.Appointments.FindAsync(id);
                if (appointment == null)
                {
                    _logger.LogWarning($"Appointment with ID {id} not found when updating status");
                    return NotFound();
                }

                appointment.Status = status;
                _context.Update(appointment);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Successfully updated appointment ID {id} status to {status}");
                TempData["SuccessMessage"] = "Randevu durumu başarıyla güncellendi.";
                return RedirectToAction(nameof(AppointmentDetails), new { id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating appointment status for ID: {id}");
                TempData["ErrorMessage"] = "Randevu durumu güncellenirken bir hata oluştu.";
                return RedirectToAction(nameof(AppointmentDetails), new { id = id });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuth]
        public async Task<IActionResult> UpdateAppointmentTime(int id, string startTime, string endTime)
        {
            try
            {
                _logger.LogInformation($"Updating appointment time for ID: {id}, Start: {startTime}, End: {endTime}");

                var appointment = await _context.Appointments
                    .Include(a => a.AppointmentServices)
                    .ThenInclude(a => a.Service)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (appointment == null)
                {
                    _logger.LogWarning($"Appointment with ID {id} not found when updating time");
                    return NotFound();
                }

                // Parse start and end times
                if (!TimeSpan.TryParse(startTime, out var parsedStartTime) || 
                    !TimeSpan.TryParse(endTime, out var parsedEndTime))
                {
                    _logger.LogWarning($"Invalid time format for appointment {id}: Start={startTime}, End={endTime}");
                    TempData["ErrorMessage"] = "Geçersiz saat formatı.";
                    return RedirectToAction(nameof(AppointmentDetails), new { id = id });
                }

                _logger.LogInformation($"Parsed times for appointment {id}: Start={parsedStartTime}, End={parsedEndTime}");

                // Validate that start is before end
                if (parsedStartTime >= parsedEndTime)
                {
                    _logger.LogWarning($"Invalid time range for appointment {id}: Start={parsedStartTime} is not before End={parsedEndTime}");
                    TempData["ErrorMessage"] = "Başlangıç saati, bitiş saatinden önce olmalıdır.";
                    return RedirectToAction(nameof(AppointmentDetails), new { id = id });
                }

                // Check if the time slot is available
                bool isAvailable = await _appointmentService.IsTimeSlotAvailableAsync(
                    appointment.AppointmentDate,
                    parsedStartTime,
                    parsedEndTime,
                    id); // Exclude this appointment from the check

                if (!isAvailable)
                {
                    _logger.LogWarning($"Time slot not available for appointment {id}: {appointment.AppointmentDate.ToShortDateString()} {parsedStartTime}-{parsedEndTime}");
                    TempData["ErrorMessage"] = "Seçilen zaman dilimi başka bir randevu ile çakışıyor.";
                    return RedirectToAction(nameof(AppointmentDetails), new { id = id });
                }

                // Update appointment times
                appointment.StartTime = parsedStartTime;
                appointment.EndTime = parsedEndTime;

                _context.Update(appointment);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Successfully updated time for appointment ID {id} to {parsedStartTime}-{parsedEndTime}");
                TempData["SuccessMessage"] = "Randevu saati başarıyla güncellendi.";
                return RedirectToAction(nameof(AppointmentDetails), new { id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating appointment time for ID: {id}");
                TempData["ErrorMessage"] = "Randevu saati güncellenirken bir hata oluştu.";
                return RedirectToAction(nameof(AppointmentDetails), new { id = id });
            }
        }

        #endregion

        #region Testimonials

        [AdminAuth]
        public async Task<IActionResult> Testimonials()
        {
            var testimonials = await _context.Testimonials
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return View(testimonials);
        }

        [AdminAuth]
        public async Task<IActionResult> TestimonialEdit(int id)
        {
            var testimonial = await _context.Testimonials.FindAsync(id);
            if (testimonial == null)
            {
                return NotFound();
            }

            return View(testimonial);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuth]
        public async Task<IActionResult> TestimonialEdit(Testimonial testimonial)
        {
            if (!ModelState.IsValid)
            {
                return View(testimonial);
            }

            _context.Update(testimonial);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Müşteri yorumu başarıyla güncellendi.";
            return RedirectToAction(nameof(Testimonials));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuth]
        public async Task<IActionResult> TestimonialDelete(int id)
        {
            var testimonial = await _context.Testimonials.FindAsync(id);
            if (testimonial == null)
            {
                return NotFound();
            }

            _context.Testimonials.Remove(testimonial);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Müşteri yorumu başarıyla silindi.";
            return RedirectToAction(nameof(Testimonials));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuth]
        public async Task<IActionResult> TestimonialApprove(int id)
        {
            var testimonial = await _context.Testimonials.FindAsync(id);
            if (testimonial == null)
            {
                return NotFound();
            }

            testimonial.IsApproved = true;
            _context.Update(testimonial);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Müşteri yorumu başarıyla onaylandı.";
            return RedirectToAction(nameof(Testimonials));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuth]
        public async Task<IActionResult> TestimonialCreate(Testimonial testimonial)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Yorum eklenirken bir hata oluştu. Lütfen tüm alanları doğru şekilde doldurun.";
                return RedirectToAction(nameof(Testimonials));
            }

            testimonial.CreatedAt = DateTime.Now;
            _context.Testimonials.Add(testimonial);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Yeni müşteri yorumu başarıyla eklendi.";
            return RedirectToAction(nameof(Testimonials));
        }

        #endregion

        #region Settings

        [AdminAuth]
        public async Task<IActionResult> Settings()
        {
            var settings = await _context.SiteSettings.FirstOrDefaultAsync();
            if (settings == null)
            {
                settings = new SiteSettings { Id = 1 };
                _context.SiteSettings.Add(settings);
                await _context.SaveChangesAsync();
            }

            return View(settings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuth]
        public async Task<IActionResult> Settings(SiteSettings settings)
        {
            if (!ModelState.IsValid)
            {
                return View(settings);
            }

            _context.Update(settings);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Ayarlar başarıyla güncellendi.";
            return RedirectToAction(nameof(Settings));
        }

        #endregion

        #region Business Hours

        [AdminAuth]
        public async Task<IActionResult> BusinessHours()
        {
            var businessHours = await _appointmentService.GetBusinessHoursAsync();
            return View(businessHours);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuth]
        public async Task<IActionResult> BusinessHoursEdit(int Id, string DayOfWeek, string OpenTime, string CloseTime, string IsOpen)
        {
            try
            {
                _logger.LogInformation($"BusinessHoursEdit called with: Id={Id}, Day={DayOfWeek}, IsOpen={IsOpen}, Open={OpenTime}, Close={CloseTime}");

                var businessHours = new BusinessHours
                {
                    Id = Id,
                    DayOfWeek = DayOfWeek,
                    // Convert checkbox value to boolean - "on" means it's checked, anything else is false
                    IsOpen = IsOpen == "on",
                    OpenTime = OpenTime,
                    CloseTime = CloseTime
                };

                _logger.LogInformation($"Parsed businessHours: Id={businessHours.Id}, Day={businessHours.DayOfWeek}, IsOpen={businessHours.IsOpen}, Open={businessHours.OpenTime}, Close={businessHours.CloseTime}");

                // Ensure the businessHours object is valid
                if (businessHours.Id <= 0 || string.IsNullOrEmpty(businessHours.DayOfWeek))
                {
                    _logger.LogWarning($"Invalid business hours data: Id={businessHours.Id}, Day={businessHours.DayOfWeek}");
                    TempData["ErrorMessage"] = "Çalışma saatleri bilgileri geçersiz.";
                    return RedirectToAction(nameof(BusinessHours));
                }

                _logger.LogInformation($"Calling _appointmentService.UpdateBusinessHoursAsync with: Id={businessHours.Id}, Day={businessHours.DayOfWeek}, IsOpen={businessHours.IsOpen}, Open={businessHours.OpenTime}, Close={businessHours.CloseTime}");

                var result = await _appointmentService.UpdateBusinessHoursAsync(businessHours);

                if (result)
                {
                    _logger.LogInformation($"Successfully updated business hours for {businessHours.DayOfWeek}");
                    TempData["SuccessMessage"] = "Çalışma saatleri başarıyla güncellendi.";
                }
                else
                {
                    _logger.LogWarning($"Failed to update business hours for {businessHours.DayOfWeek}");
                    TempData["ErrorMessage"] = "Çalışma saatleri güncellenirken bir hata oluştu.";
                }

                return RedirectToAction(nameof(BusinessHours));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating business hours for ID {Id}");
                TempData["ErrorMessage"] = "Çalışma saatleri güncellenirken bir hata oluştu: " + ex.Message;
                return RedirectToAction(nameof(BusinessHours));
            }
        }

        #endregion

        #region Telegram

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuth]
        [Consumes("application/json")]
        public async Task<IActionResult> TestTelegram([FromBody] TelegramTestModel model)
        {
            try
            {
                _logger.LogInformation($"Received model: {(model != null ? "Valid model" : "Null model")}");

                if (model == null)
                {
                    return Json(new { success = false, message = "Model null geldi. Geçersiz JSON formatı olabilir." });
                }

                if (string.IsNullOrEmpty(model.ApiKey))
                {
                    return Json(new { success = false, message = "API anahtarı gereklidir." });
                }

                // Log the input values for debugging
                _logger.LogInformation($"Testing Telegram with ApiKey: {model.ApiKey}");

                try
                {
                    // Use the provided API key and chat ID for testing
                    string apiKey = model.ApiKey;
                    string chatId = model.ChatId;

                    // Get current settings
                    var settings = await _context.SiteSettings.FirstOrDefaultAsync();
                    if (settings == null)
                    {
                        return Json(new { success = false, message = "Site ayarları bulunamadı." });
                    }

                    // Update the API key and chat ID
                    settings.TelegramApiKey = apiKey;
                    settings.TelegramChatId = chatId;
                    await _context.SaveChangesAsync();

                    // Use the telegram service to send the test message to all configured chat IDs
                    await _telegramService.SendTestMessageAsync("Fatma Hanım Botunuz Aktif Bol Kazançlar");

                    _logger.LogInformation("Test message sent successfully");
                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error sending test message");
                    return Json(new { success = false, message = ex.Message });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error testing Telegram");
                return Json(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Financial Records

        [AdminAuth]
        public async Task<IActionResult> FinancialRecords(DateTime? startDate, DateTime? endDate)
        {
            var start = startDate ?? DateTime.Today.AddDays(-30);
            var end = endDate ?? DateTime.Today;

            var records = await _context.FinancialRecords
                .Include(f => f.Appointment)
                .Where(f => f.Date >= start && f.Date <= end)
                .OrderByDescending(f => f.Date)
                .ToListAsync();

            var viewModel = new FinancialRecordsViewModel
            {
                Records = records,
                StartDate = start,
                EndDate = end,
                TotalIncome = records.Where(r => r.Type == FinancialRecordType.Income).Sum(r => r.Amount),
                TotalExpense = records.Where(r => r.Type == FinancialRecordType.Expense).Sum(r => r.Amount)
            };

            return View(viewModel);
        }

        [AdminAuth]
        public IActionResult FinancialRecordCreate()
        {
            return View(new FinancialRecord { Date = DateTime.Today });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuth]
        public async Task<IActionResult> FinancialRecordCreate(FinancialRecord record)
        {
            if (!ModelState.IsValid)
            {
                return View(record);
            }

            _context.FinancialRecords.Add(record);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Finansal kayıt başarıyla eklendi.";
            return RedirectToAction(nameof(FinancialRecords));
        }

        [AdminAuth]
        public async Task<IActionResult> FinancialRecordEdit(int id)
        {
            var record = await _context.FinancialRecords.FindAsync(id);
            if (record == null)
            {
                return NotFound();
            }

            return View(record);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuth]
        public async Task<IActionResult> FinancialRecordEdit(FinancialRecord record)
        {
            if (!ModelState.IsValid)
            {
                return View(record);
            }

            _context.FinancialRecords.Update(record);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Finansal kayıt başarıyla güncellendi.";
            return RedirectToAction(nameof(FinancialRecords));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuth]
        public async Task<IActionResult> FinancialRecordDelete(int id)
        {
            var record = await _context.FinancialRecords.FindAsync(id);
            if (record != null)
            {
                _context.FinancialRecords.Remove(record);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Finansal kayıt başarıyla silindi.";
            }

            return RedirectToAction(nameof(FinancialRecords));
        }

        #endregion

        #region Documents

        [AdminAuth]
        public async Task<IActionResult> Documents()
        {
            var documents = await _context.Documents
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();

            return View(documents);
        }

        [AdminAuth]
        public IActionResult DocumentCreate()
        {
            return View(new BusinessDocument());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuth]
        public async Task<IActionResult> DocumentCreate(BusinessDocument document)
        {
            if (!ModelState.IsValid)
            {
                return View(document);
            }

            _context.Documents.Add(document);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Belge başarıyla eklendi.";
            return RedirectToAction(nameof(Documents));
        }

        [AdminAuth]
        public async Task<IActionResult> DocumentEdit(int id)
        {
            var document = await _context.Documents.FindAsync(id);
            if (document == null)
            {
                return NotFound();
            }

            return View(document);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuth]
        public async Task<IActionResult> DocumentEdit(BusinessDocument document)
        {
            if (!ModelState.IsValid)
            {
                return View(document);
            }

            _context.Documents.Update(document);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Belge başarıyla güncellendi.";
            return RedirectToAction(nameof(Documents));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuth]
        public async Task<IActionResult> DocumentComplete(int id)
        {
            var document = await _context.Documents.FindAsync(id);
            if (document != null)
            {
                document.IsCompleted = true;
                document.CompletedAt = DateTime.Now;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Belge tamamlandı olarak işaretlendi.";
            }

            return RedirectToAction(nameof(Documents));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuth]
        public async Task<IActionResult> DocumentDelete(int id)
        {
            var document = await _context.Documents.FindAsync(id);
            if (document != null)
            {
                _context.Documents.Remove(document);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Belge başarıyla silindi.";
            }

            return RedirectToAction(nameof(Documents));
        }

        #endregion

        #region Helpers

        private string GenerateSlug(string text)
        {
            // Convert to lowercase
            text = text.ToLowerInvariant();

            // Replace Turkish characters
            text = text.Replace("ı", "i")
                       .Replace("ğ", "g")
                       .Replace("ü", "u")
                       .Replace("ş", "s")
                       .Replace("ö", "o")
                       .Replace("ç", "c");

            // Remove special characters
            text = System.Text.RegularExpressions.Regex.Replace(text, @"[^a-z0-9\s-]", "");

            // Replace spaces with hyphens
            text = System.Text.RegularExpressions.Regex.Replace(text, @"\s+", "-");

            // Remove multiple hyphens
            text = System.Text.RegularExpressions.Regex.Replace(text, @"-+", "-");

            // Trim hyphens from start and end
            text = text.Trim('-');

            return text;
        }

        #endregion
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "Şifre gereklidir.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }

    public class DashboardViewModel
    {
        public List<Appointment> UpcomingAppointments { get; set; } = new List<Appointment>();
        public int ServiceCount { get; set; }
        public int PendingTestimonialCount { get; set; }
        public decimal MonthlyIncome { get; set; }
        public decimal MonthlyExpense { get; set; }
        public decimal MonthlyProfit { get; set; }
        public decimal AppointmentIncome { get; set; }
        public int PendingDocuments { get; set; }
        public int HighPriorityDocuments { get; set; }
        public string CurrentMonth { get; set; } = string.Empty;
    }

    public class ServiceEditViewModel
    {
        public Service Service { get; set; } = new Service();
        public List<ServiceCategory> Categories { get; set; } = new List<ServiceCategory>();
    }

    public class AppointmentsViewModel
    {
        public DateTime Date { get; set; }
        public List<Appointment> Appointments { get; set; } = new List<Appointment>();
    }

    public class TelegramTestModel
    {
        public string ApiKey { get; set; } = string.Empty;
        public string ChatId { get; set; } = string.Empty;
    }

    public class FinancialRecordsViewModel
    {
        public List<FinancialRecord> Records { get; set; } = new List<FinancialRecord>();
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal NetProfit => TotalIncome - TotalExpense;
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AdminAuthAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.Session.GetString("IsAdmin") != "true")
            {
                context.Result = new RedirectToActionResult("Login", "Admin", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
