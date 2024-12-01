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
		public IActionResult QuanLySanPham()
		{
			return View();
		}
	
		public IActionResult ThemSanPham()
		{
			return View();
		}
		
		public IActionResult SuaSanPham()
		{
			return View();
		}
	}
}
