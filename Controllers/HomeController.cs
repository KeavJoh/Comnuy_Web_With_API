using ComnuyWebWithAPI.Data;
using ComnuyWebWithAPI.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ComnuyWebWithAPI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            List<Tool> toolsFromDb = _context.Tools.ToList();
            List<Tool> toolOrderByDateFromDb = _context.Tools.OrderByDescending(x => x.LastChangesDate).Take(4).ToList();
            Random random = new();
            var randomTools = toolsFromDb.OrderBy(x => random.Next()).Take(4).ToList();

            var model = new _MyContentViewModel
            {
                RandomTools = randomTools,
                ToolsOrderByDate = toolOrderByDateFromDb
            };
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}