using Microsoft.AspNetCore.Mvc;

namespace QuizeManagement.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult DashboardView()
        {
            return View();
        }
    }
}
