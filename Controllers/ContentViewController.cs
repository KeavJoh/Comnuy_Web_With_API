using ComnuyWebWithAPI.Data;
using ComnuyWebWithAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
            var toolGroups = _context.ToolGroups.ToList();
            ViewBag.ToolGroups = toolGroups;
            return View();
        }

        //public IActionResult SubmitCreateOrEditTool(Tool tool)
        //{
        //    tool.Owner = User.Identity.Name;
        //}
    }
}
