using Microsoft.AspNetCore.Mvc;

namespace KitchenCompanionWebApi.Controllers
{
    public class AIController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
