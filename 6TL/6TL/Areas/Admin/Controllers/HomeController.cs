using Microsoft.AspNetCore.Mvc;

namespace _6TL.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
	{
		
		public IActionResult Index()
		{
			return View();
		}
        public IActionResult QlBlog()
        {
            return View();
        }
    }
}
