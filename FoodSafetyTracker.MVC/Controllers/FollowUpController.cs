using Microsoft.AspNetCore.Mvc;

namespace FoodSafetyTracker.MVC.Controllers
{
    public class FollowUpController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
