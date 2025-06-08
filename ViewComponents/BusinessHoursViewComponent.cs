using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinaKuafor.Data;
using TinaKuafor.Models;

namespace TinaKuafor.ViewComponents
{
    public class BusinessHoursViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BusinessHoursViewComponent> _logger;

        public BusinessHoursViewComponent(ApplicationDbContext context, ILogger<BusinessHoursViewComponent> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            try
            {
                _logger.LogInformation("Loading business hours for view component");

                // First fetch all business hours from database
                var businessHours = await _context.BusinessHours
                    .ToListAsync();

                // Then order them in memory
                var orderedBusinessHours = businessHours
                    .OrderBy(b => GetDayOrder(b.DayOfWeek))
                    .ToList();

                if (orderedBusinessHours.Count == 0)
                {
                    _logger.LogWarning("No business hours found");
                    return View("Default", new List<BusinessHours>());
                }

                _logger.LogInformation($"Loaded {businessHours.Count} business hours records");
                return View("Default", orderedBusinessHours);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading business hours");
                return View("Default", new List<BusinessHours>());
            }
        }

        private int GetDayOrder(string day)
        {
            switch (day)
            {
                case "Monday": return 1;
                case "Tuesday": return 2;
                case "Wednesday": return 3;
                case "Thursday": return 4;
                case "Friday": return 5;
                case "Saturday": return 6;
                case "Sunday": return 7;
                default: return 0;
            }
        }
    }
} 