﻿using ComnuyWebWithAPI.Data;
using ComnuyWebWithAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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

        public IActionResult Index(int selectedType)
        {
            List<Tool> toolsFromDb;
            List<ToolGroup> toolGroupsFromDb;

            if (selectedType == 1)
            {
                toolsFromDb = _context.Tools.ToList();
            } else
            {
                toolsFromDb = _context.Tools.Where(x => x.ToolGroup == selectedType).ToList();
            }
            toolGroupsFromDb = _context.ToolGroups.ToList();

            var model = new _ContentDeliveryModel
            {
                Tools = toolsFromDb,
                ToolGroups = toolGroupsFromDb,
                SelectedToolGroupId = selectedType

            };
            return View(model);
        }

        public IActionResult ShowToolDetail(int toolId)
        {
            Tool toolFromDb;
            ToolGroup toolGroup;
            bool checkIfFavoriteExist;

            if (toolId != 0)
            {
                toolFromDb = _context.Tools.SingleOrDefault(x => x.Id == toolId);

                if (toolFromDb != null)
                {
                    checkIfFavoriteExist = _context.ToolFavoritesFromUsers.Any(x => x.ToolId == toolId && x.UserName == User.Identity.Name);
                    toolGroup = _context.ToolGroups.SingleOrDefault(x => x.Id == toolFromDb.ToolGroup);
                    var model = new _ContentDeliveryModel
                    {
                        Tool = toolFromDb,
                        ToolGroup = toolGroup,
                        IsFavoriteForUser = checkIfFavoriteExist
                    };
                    return View(model);
                } else
                {
                    return NotFound();
                }
            } 

            return View();
        }

        [Authorize]
        public IActionResult MyContent()
        {
            var toolsFromDb = _context.Tools.Where(x => x.Owner == User.Identity.Name).ToList();
            var toolGroupsFromDb = _context.ToolGroups.ToList();

            var model = new _ContentDeliveryModel
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

                    int nextDbId;
                    if (_context.Tools.Any())
                    {
                        nextDbId = _context.Tools.Max(t => t.Id) + 1;
                    }
                    else
                    {
                        nextDbId = 1;
                    }
                    string nextDbIdString = nextDbId.ToString();

                    string toolFolderString = nextDbIdString;

                    uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Pictures\\Tool\\");
                    toolFolder = Path.Combine(uploadsFolder, toolFolderString);
                    Directory.CreateDirectory(toolFolder);

                    fileExtension = Path.GetExtension(file.FileName);

                    uniqueFileName = $"{tool.Name}_{nextDbIdString}{fileExtension}";
                    filePath = Path.Combine(toolFolder, uniqueFileName);


                    using (var fileStream = file.OpenReadStream())
                    {
                        using (Image image = Image.Load(fileStream))
                        {
                            // Hier wird das Bild auf die Zielgröße skaliert (kann je nach Bedarf angepasst werden)
                            int targetWidth = 250;
                            int targetHeight = 225;
                            image.Mutate(x => x.Resize(new ResizeOptions
                            {
                                Mode = ResizeMode.BoxPad,
                                Size = new Size(targetWidth, targetHeight)
                            }));

                            // Speichern des komprimierten Bildes als PNG
                            using (var pngFileStream = new FileStream(filePath, FileMode.Create))
                            {
                                image.SaveAsPng(pngFileStream);
                            }
                        }
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

        [Authorize]
        [HttpPost]
        public IActionResult DeleteToolFromDb(int id)
        {
            if (id != 0)
            {
                var toolPostingFromDb = _context.Tools.SingleOrDefault(x => x.Id == id);

                if (toolPostingFromDb.Owner != User.Identity.Name)
                {
                    return Unauthorized();
                } else if (toolPostingFromDb.Owner == null)
                {
                    return Unauthorized();
                }

                if (toolPostingFromDb != null)
                {

                    if ( toolPostingFromDb.Picture_1 != "\\Pictures\\Tool\\Placeholder\\placeholder.png")
                    {
                        string webRootPath = _webHostEnvironment.WebRootPath;
                        string absoluteImagePath = Path.Combine(webRootPath + "\\Pictures\\Tool\\" + toolPostingFromDb.Id);
                        if (Directory.Exists(absoluteImagePath))
                        {
                            Directory.Delete(absoluteImagePath, recursive: true);
                        }
                    }

                    _context.Tools.Remove(toolPostingFromDb);
                    _context.SaveChanges();
                } else
                {
                    return NotFound();
                }
            } else
            {
                return BadRequest();
            }

            return Ok();
        }

        [Authorize]
        public IActionResult FavoritesContentTools()
        {
            var favoriteToolIds = _context.ToolFavoritesFromUsers
                .Where(x => x.UserName == User.Identity.Name)
                .Select(x => x.ToolId)
                .ToList();
            var favoriteToolsFromDb = _context.Tools.Where(tool => favoriteToolIds.Contains(tool.Id)).ToList();
            var toolGroupsFromDb = _context.ToolGroups.ToList();

            var model = new _ContentDeliveryModel
            {
                Tools = favoriteToolsFromDb,
                ToolGroups = toolGroupsFromDb
            };

            return View(model);
        }

        [Authorize]
        public IActionResult CheckAndSetFavoriteTool(int toolId)
        {
            var currentUser = User.Identity.Name;
            bool checkIfFavoriteExist = _context.ToolFavoritesFromUsers.Any(x => x.ToolId == toolId && x.UserName == currentUser);

            if (checkIfFavoriteExist)
            {
                var exisitngFavoriteTool = _context.ToolFavoritesFromUsers.FirstOrDefault(x => x.ToolId == toolId && x.UserName == currentUser);               

                _context.ToolFavoritesFromUsers.Remove(exisitngFavoriteTool);
                _context.SaveChanges();

                return RedirectToAction("ShowToolDetail", new { toolId = toolId });
            }
            else
            {
                var newToolFavorite = new ToolFavorites
                {
                    ToolId = toolId,
                    UserName = currentUser
                };

                _context.ToolFavoritesFromUsers.Add(newToolFavorite);
                _context.SaveChanges();

                return RedirectToAction("ShowToolDetail", new { toolId = toolId });
            }
        }
    }
}
