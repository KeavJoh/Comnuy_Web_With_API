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
            string uploadsFolder;
            string toolFolder;
            string fileExtension;
            string uniqueFileName;
            string filePath;

            tool.LastChangesDate = DateTime.UtcNow;
            tool.LastChangeUser = User.Identity.Name;

            if (toolId == 0)
            {
                if (file != null)
                {
                    var nextDbId = _context.Tools.Max(t => t.Id) + 1;
                    string nextDbIdString = nextDbId.ToString();

                    string toolFolderString = nextDbIdString;

                    uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Pictures\\Tool\\");
                    toolFolder = Path.Combine(uploadsFolder, toolFolderString);
                    Directory.CreateDirectory(toolFolder);

                    fileExtension = Path.GetExtension(file.FileName);

                    uniqueFileName = $"{tool.Name}_{nextDbIdString}{fileExtension}";
                    filePath = Path.Combine(toolFolder, uniqueFileName);
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
                    uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Pictures\\Tool\\");
                    toolFolder = Path.Combine(uploadsFolder, toolFromDbId);
                    fileExtension = Path.GetExtension(file.FileName);
                    uniqueFileName = $"{tool.Name}_{toolFromDbId}{fileExtension}";
                    filePath = Path.Combine(toolFolder, uniqueFileName);
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
                toolFromDb.LastChangeUser = tool.LastChangeUser;
                toolFromDb.LastChangesDate = DateTime.UtcNow;


            }

            _context.SaveChanges();

            return RedirectToAction("MyContent");
        }
    }
}
