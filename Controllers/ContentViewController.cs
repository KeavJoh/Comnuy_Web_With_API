using ComnuyWebWithAPI.Data;
using ComnuyWebWithAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.EntityFrameworkCore;

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
        public IActionResult CreateOrEditTool(int id)
        {
            var toolGroups = _context.ToolGroups.ToList();

            if (id != 0)
            {
                var toolPostingFromDb = _context.Tools.SingleOrDefault(x => x.Id == id);
                if (toolPostingFromDb.Owner != User.Identity.Name)
                {
                    return Unauthorized();
                }

                if (toolPostingFromDb != null)
                {
                    ViewBag.ToolGroups = toolGroups;
                    return View(toolPostingFromDb);
                }
                else
                {
                    return NotFound();
                }
            }

            ViewBag.ToolGroups = toolGroups;
            return View();
        }

        [Authorize]
        public IActionResult SubmitCreateOrEditTool(Tool tool, IFormFile file)
        {
            var toolId = tool.Id;
            tool.LastChangesDate = DateTime.UtcNow;

            if (toolId == 0)
            {
                if (file != null)
                {
                    var nextDbId = _context.Tools.Max(t => t.Id) + 1;
                    string nextDbIdString = nextDbId.ToString();

                    string toolFolderString = nextDbIdString;

                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Pictures\\Tool\\");
                    string toolFolder = Path.Combine(uploadsFolder, toolFolderString);
                    Directory.CreateDirectory(toolFolder);

                    string fileExtension = Path.GetExtension(file.FileName);

                    string uniqueFileName = $"{tool.Name}_{nextDbIdString}{fileExtension}";
                    string filePath = Path.Combine(toolFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    tool.Picture_1 = Path.Combine("\\Pictures\\Tool\\", toolFolderString, uniqueFileName);
                }
                else if (file == null)
                {
                    tool.Picture_1 = Path.Combine("\\Pictures\\Tool\\Placeholder\\placeholder.png");
                }

                tool.Owner = User.Identity.Name;
                _context.Tools.Add(tool);

            } else if (toolId != 0) 
            {
                var toolFromDb = _context.Tools.FirstOrDefault(x => x.Id == tool.Id);
                string toolFromDbId = toolFromDb.Id.ToString();

                if (file != null && (toolFromDb.Picture_1 != tool.Picture_1))
                {
                    string deleteFilePath = Path.Combine(_webHostEnvironment.WebRootPath, toolFromDb.Picture_1);
                    FileInfo fileInfo = new FileInfo(deleteFilePath);
                    if (fileInfo.Exists)
                    {
                        fileInfo.Delete();
                    }
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Pictures\\Tool\\");
                    string toolFolder = Path.Combine(uploadsFolder, toolFromDbId);
                    string fileExtension = Path.GetExtension(file.FileName);
                    string uniqueFileName = $"{tool.Name}_{toolFromDbId}{fileExtension}";
                    string filePath = Path.Combine(toolFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    toolFromDb.Picture_1 = Path.Combine("\\Pictures\\Tool\\", toolFromDbId, uniqueFileName);
                }

                toolFromDb.Name = tool.Name;
                toolFromDb.Description = tool.Description;
                toolFromDb.ShortDescription = tool.ShortDescription;
                toolFromDb.WebAddress = tool.WebAddress;
                toolFromDb.ToolGroup = tool.ToolGroup;
                toolFromDb.LastChangeUser = User.Identity.Name;
                toolFromDb.LastChangesDate = DateTime.UtcNow;


            }

            _context.SaveChanges();

            return RedirectToAction("MyContent");
        }
    }
}
