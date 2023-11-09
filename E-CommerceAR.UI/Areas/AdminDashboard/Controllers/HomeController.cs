using Microsoft.AspNetCore.Mvc;

namespace E_CommerceAR.UI.Areas.AdminDashboard.Controllers
{
    [Area("AdminDashboard")]
    public class HomeController : Controller
    {
        [HttpGet]
        [Route("AdminDashboard/Home/Index")]
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Menu()
        {
            return PartialView();
        }
        public IActionResult NavBar()
        {
            return PartialView();
        }
    }
}
