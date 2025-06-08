using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TinaKuafor.Data;
using TinaKuafor.Models;
using System.Xml;
using System.Text;

namespace TinaKuafor.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var settings = await _context.SiteSettings.FirstOrDefaultAsync();
            var testimonials = await _context.Testimonials
                .Where(t => t.IsApproved)
                .OrderBy(t => t.DisplayOrder)
                .ToListAsync();

            var viewModel = new HomeViewModel
            {
                SiteSettings = settings ?? new SiteSettings(),
                Testimonials = testimonials
            };

            return View(viewModel);
        }

        public async Task<IActionResult> About()
        {
            var settings = await _context.SiteSettings.FirstOrDefaultAsync();
            return View(settings ?? new SiteSettings());
        }

        public async Task<IActionResult> Contact()
        {
            var settings = await _context.SiteSettings.FirstOrDefaultAsync();
            return View("~/Views/Contact/Index.cshtml", settings ?? new SiteSettings());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult TermsOfService()
        {
            return View();
        }

        public IActionResult PrivacyPolicy()
        {
            return View();
        }

        public IActionResult CookiePolicy()
        {
            return View();
        }

        [Route("sitemap.xml")]
        public IActionResult Sitemap()
        {
            // Return the XML file
            return File("~/sitemap.xml", "application/xml");
        }

        [Route("robots.txt")]
        public IActionResult Robots()
        {
            // Return the robots.txt file
            return File("~/robots.txt", "text/plain");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public class HomeViewModel
    {
        public SiteSettings SiteSettings { get; set; } = new SiteSettings();
        public List<Testimonial> Testimonials { get; set; } = new List<Testimonial>();
    }

    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}