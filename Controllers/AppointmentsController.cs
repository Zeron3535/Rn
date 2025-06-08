using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinaKuafor.Data;
using TinaKuafor.Models;
using TinaKuafor.Services;

namespace TinaKuafor.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAppointmentService _appointmentService;

        public AppointmentsController(
            ApplicationDbContext context,
            IAppointmentService appointmentService)
        {
            _context = context;
            _appointmentService = appointmentService;
        }

        public async Task<IActionResult> Index()
        {
            var services = await _context.Services
                .Where(s => s.IsActive)
                .OrderBy(s => s.CategoryId)
                .ThenBy(s => s.Name)
                .ToListAsync();

            var categories = await _context.ServiceCategories
                .Where(c => c.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync();

            // Get current date and time in Turkey's time zone
            var now = TimeService.Now;

            // Set default appointment date to tomorrow or next available day
            var defaultDate = now.Date.AddDays(1);

            // Check if tomorrow is available, if not, find the next available day
            while (!await _appointmentService.IsDayAvailableForBookingAsync(defaultDate))
            {
                defaultDate = defaultDate.AddDays(1);
            }

            var viewModel = new AppointmentViewModel
            {
                Services = services,
                Categories = categories,
                Appointment = new Appointment
                {
                    AppointmentDate = defaultDate
                }
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppointmentViewModel model, List<int> selectedServices)
        {
            if (!ModelState.IsValid)
            {
                model.Services = await _context.Services
                    .Where(s => s.IsActive)
                    .OrderBy(s => s.CategoryId)
                    .ThenBy(s => s.Name)
                    .ToListAsync();

                model.Categories = await _context.ServiceCategories
                    .Where(c => c.IsActive)
                    .OrderBy(c => c.DisplayOrder)
                    .ToListAsync();

                return View("Index", model);
            }

            if (selectedServices == null || !selectedServices.Any())
            {
                ModelState.AddModelError("", "Lütfen en az bir hizmet seçin.");

                model.Services = await _context.Services
                    .Where(s => s.IsActive)
                    .OrderBy(s => s.CategoryId)
                    .ThenBy(s => s.Name)
                    .ToListAsync();

                model.Categories = await _context.ServiceCategories
                    .Where(c => c.IsActive)
                    .OrderBy(c => c.DisplayOrder)
                    .ToListAsync();

                return View("Index", model);
            }

            try
            {
                // Get current date and time in Turkey's time zone
                var now = TimeService.Now;

                // Check if the appointment date is in the past
                if (model.Appointment.AppointmentDate.Date < now.Date)
                {
                    ModelState.AddModelError("", "Geçmiş tarihlere randevu alınamaz.");

                    model.Services = await _context.Services
                        .Where(s => s.IsActive)
                        .OrderBy(s => s.CategoryId)
                        .ThenBy(s => s.Name)
                        .ToListAsync();

                    model.Categories = await _context.ServiceCategories
                        .Where(c => c.IsActive)
                        .OrderBy(c => c.DisplayOrder)
                        .ToListAsync();

                    return View("Index", model);
                }

                // Check if the appointment time is in the past for today (with 15 minute buffer)
                if (model.Appointment.AppointmentDate.Date == now.Date && model.Appointment.StartTime <= now.TimeOfDay.Add(TimeSpan.FromMinutes(15)))
                {
                    ModelState.AddModelError("", "Geçmiş saatlere veya çok yakın saatlere randevu alınamaz. Lütfen en az 15 dakika sonrası için randevu alın.");

                    model.Services = await _context.Services
                        .Where(s => s.IsActive)
                        .OrderBy(s => s.CategoryId)
                        .ThenBy(s => s.Name)
                        .ToListAsync();

                    model.Categories = await _context.ServiceCategories
                        .Where(c => c.IsActive)
                        .OrderBy(c => c.DisplayOrder)
                        .ToListAsync();

                    return View("Index", model);
                }

                // Calculate total duration
                var services = await _context.Services
                    .Where(s => selectedServices.Contains(s.Id) && s.IsActive)
                    .ToListAsync();

                if (services.Count != selectedServices.Count)
                {
                    ModelState.AddModelError("", "Bazı seçilen hizmetler bulunamadı veya aktif değil.");

                    model.Services = await _context.Services
                        .Where(s => s.IsActive)
                        .OrderBy(s => s.CategoryId)
                        .ThenBy(s => s.Name)
                        .ToListAsync();

                    model.Categories = await _context.ServiceCategories
                        .Where(c => c.IsActive)
                        .OrderBy(c => c.DisplayOrder)
                        .ToListAsync();

                    return View("Index", model);
                }

                int totalDurationMinutes = services.Sum(s => s.DurationMinutes);

                // Set end time based on start time and duration
                model.Appointment.EndTime = model.Appointment.StartTime.Add(TimeSpan.FromMinutes(totalDurationMinutes));

                // Check if the time slot is available
                bool isAvailable = await _appointmentService.IsTimeSlotAvailableAsync(
                    model.Appointment.AppointmentDate,
                    model.Appointment.StartTime,
                    model.Appointment.EndTime);

                if (!isAvailable)
                {
                    ModelState.AddModelError("", "Seçilen zaman dilimi müsait değil. Lütfen başka bir zaman seçin.");

                    model.Services = await _context.Services
                        .Where(s => s.IsActive)
                        .OrderBy(s => s.CategoryId)
                        .ThenBy(s => s.Name)
                        .ToListAsync();

                    model.Categories = await _context.ServiceCategories
                        .Where(c => c.IsActive)
                        .OrderBy(c => c.DisplayOrder)
                        .ToListAsync();

                    return View("Index", model);
                }

                // Create the appointment
                var appointment = await _appointmentService.CreateAppointmentAsync(model.Appointment, selectedServices);

                TempData["SuccessMessage"] = "Randevunuz başarıyla oluşturuldu. Teşekkür ederiz!";
                return RedirectToAction(nameof(Confirmation), new { id = appointment.Id });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);

                model.Services = await _context.Services
                    .Where(s => s.IsActive)
                    .OrderBy(s => s.CategoryId)
                    .ThenBy(s => s.Name)
                    .ToListAsync();

                model.Categories = await _context.ServiceCategories
                    .Where(c => c.IsActive)
                    .OrderBy(c => c.DisplayOrder)
                    .ToListAsync();

                return View("Index", model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Randevu oluşturulurken bir hata oluştu: " + ex.Message);

                model.Services = await _context.Services
                    .Where(s => s.IsActive)
                    .OrderBy(s => s.CategoryId)
                    .ThenBy(s => s.Name)
                    .ToListAsync();

                model.Categories = await _context.ServiceCategories
                    .Where(c => c.IsActive)
                    .OrderBy(c => c.DisplayOrder)
                    .ToListAsync();

                return View("Index", model);
            }
        }

        public async Task<IActionResult> Confirmation(int id)
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableTimeSlots(DateTime date, List<int> serviceIds)
        {
            // Get current date and time in Turkey's time zone
            var now = TimeService.Now;

            // Check if the date is in the past
            if (date.Date < now.Date)
            {
                return Json(new { error = "Geçmiş tarihlere randevu alınamaz." });
            }

            // Check if the day is available for booking
            if (!await _appointmentService.IsDayAvailableForBookingAsync(date))
            {
                return Json(new { error = "Bu gün işletme kapalıdır, randevu alınamaz." });
            }

            if (serviceIds == null || !serviceIds.Any())
            {
                // If no services selected, return empty list with message
                return Json(new { error = "Lütfen en az bir hizmet seçin." });
            }

            var timeSlots = await _appointmentService.GetAvailableTimeSlotsAsync(date, serviceIds);
            return Json(timeSlots);
        }

        [HttpGet]
        public async Task<IActionResult> IsDayAvailable(DateTime date)
        {
            // Get current date and time in Turkey's time zone
            var now = TimeService.Now;

            // Check if the date is in the past
            if (date.Date < now.Date)
            {
                return Json(new { isAvailable = false, message = "Geçmiş tarihlere randevu alınamaz." });
            }

            var isAvailable = await _appointmentService.IsDayAvailableForBookingAsync(date);

            string message = isAvailable 
                ? "Bu gün randevu alınabilir." 
                : "Bu gün işletme kapalıdır, randevu alınamaz.";

            return Json(new { isAvailable, message });
        }
    }

    public class AppointmentViewModel
    {
        public Appointment Appointment { get; set; } = new Appointment();
        public List<Service> Services { get; set; } = new List<Service>();
        public List<ServiceCategory> Categories { get; set; } = new List<ServiceCategory>();
        public List<int> SelectedServices { get; set; } = new List<int>();
    }
}
