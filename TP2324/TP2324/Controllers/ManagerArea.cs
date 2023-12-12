using Microsoft.AspNetCore.Mvc;

namespace TP2324.Controllers
{
    public class ManagerArea : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
