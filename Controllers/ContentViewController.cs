using ComnuyWebWithAPI.Data;
using ComnuyWebWithAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace ComnuyWebWithAPI.Controllers
{
    public class ContentViewController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ContentViewController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult MyContent()
        {
            var toolsFromDb = _context.Tools.Where(x => x.Owner == User.Identity.Name).ToList();
            var toolGroupsFromDb = _context.ToolGroups.ToList();

            var model = new _MyContentViewModel
            {
                Tools = toolsFromDb,
                ToolGroups = toolGroupsFromDb

            };
            return View(model);
        }

        [Authorize]
        public IActionResult CreateOrEditTool()
        {
            var toolGroups = _context.ToolGroups.ToList();
            ViewBag.ToolGroups = toolGroups;
            return View();
        }

        [Authorize]
        public IActionResult SubmitCreateOrEditTool(Tool tool, IFormFile file)
        {
            tool.Owner = User.Identity.Name;
            tool.LastChangesDate = DateTime.UtcNow;

            if (file != null)
            {
                int guidLenghtFolder = 6;
                int guidLenghtPicture = 4;

                string generatedGuidFolder = Guid.NewGuid().ToString("N").Substring(0, guidLenghtFolder);
                string generatedGuidPicture = Guid.NewGuid().ToString("N").Substring(0, guidLenghtPicture);

                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Pictures\\Tool\\");
                string toolFolder = Path.Combine(uploadsFolder, tool.ToolGroup + "_" + generatedGuidFolder + "_" + tool.Name);
                Directory.CreateDirectory(toolFolder);
                string uniqueFileName = tool.Name + "_" + generatedGuidPicture;
                string filePath = Path.Combine(toolFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                tool.Picture_1 = Path.Combine("Pictures/Tool/", toolFolder, uniqueFileName);
            }

            _context.Tools.Add(tool);
            _context.SaveChanges();

            return RedirectToAction("MyContent");
        }
    }
}
