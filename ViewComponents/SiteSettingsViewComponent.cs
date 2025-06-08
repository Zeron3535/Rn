using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinaKuafor.Data;
using TinaKuafor.Models;

namespace TinaKuafor.ViewComponents
{
    public class SiteSettingsViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SiteSettingsViewComponent> _logger;

        public SiteSettingsViewComponent(ApplicationDbContext context, ILogger<SiteSettingsViewComponent> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            try
            {
                var settings = await _context.SiteSettings.FirstOrDefaultAsync();
                
                // Set ViewBag for easy access in _Layout
                ViewBag.SiteSettings = settings ?? new SiteSettings();
                
                return View(settings ?? new SiteSettings());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading site settings");
                return View(new SiteSettings());
            }
        }
    }
} 