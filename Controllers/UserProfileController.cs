using Microsoft.AspNetCore.Mvc;

namespace QuizeManagement.Controllers
{
    public class UserProfileController : Controller
    {
        public IActionResult UserProfile()
        {
            return View();
        }
        public IActionResult UserFAQ()
        {
            return View();
        }
    }
}
