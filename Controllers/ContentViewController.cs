using Microsoft.AspNetCore.Mvc;

namespace ComnuyWebWithAPI.Controllers
{
    public class ContentViewController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
