using ComnuyWebWithAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ComnuyWebWithAPI.Controllers
{
    public class ContentViewController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContentViewController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult MyContent()
        {
            return View();
        }

        [Authorize]
        public IActionResult CreateOrEditTool()
        {
            var toolGroups = GetToolGroups();
            ViewBag.ToolGroups = new SelectList(toolGroups);
            return View();
        }

        private List<string> GetToolGroups()
        {
            var toolGroups = _context.ToolGroups.Select(t => t.Name).ToList();
            return toolGroups;
        }
    }
}
