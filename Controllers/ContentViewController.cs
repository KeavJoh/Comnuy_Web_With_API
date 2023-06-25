using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ComnuyWebWithAPI.Controllers
{
    public class ContentViewController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult MyContent()
        {
            return View();
        }
    }
}
