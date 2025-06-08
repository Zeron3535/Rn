using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinaKuafor.Data;
using TinaKuafor.Models;

namespace TinaKuafor.Controllers
{
    public class ServicesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ServicesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _context.ServiceCategories
                .Include(c => c.Services.Where(s => s.IsActive))
                .Where(c => c.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync();

            return View(categories);
        }

        public async Task<IActionResult> Category(string slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return RedirectToAction(nameof(Index));
            }

            var category = await _context.ServiceCategories
                .Include(c => c.Services.Where(s => s.IsActive))
                .FirstOrDefaultAsync(c => c.UrlSlug == slug && c.IsActive);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        public async Task<IActionResult> Details(string slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return RedirectToAction(nameof(Index));
            }

            var service = await _context.Services
                .Include(s => s.Category)
                .FirstOrDefaultAsync(s => s.UrlSlug == slug && s.IsActive);

            if (service == null)
            {
                return NotFound();
            }

            // Get related services in the same category
            var relatedServices = await _context.Services
                .Where(s => s.CategoryId == service.CategoryId && s.Id != service.Id && s.IsActive)
                .Take(4)
                .ToListAsync();

            var viewModel = new ServiceDetailsViewModel
            {
                Service = service,
                RelatedServices = relatedServices
            };

            return View(viewModel);
        }
    }

    public class ServiceDetailsViewModel
    {
        public Service Service { get; set; } = null!;
        public List<Service> RelatedServices { get; set; } = new List<Service>();
    }
}