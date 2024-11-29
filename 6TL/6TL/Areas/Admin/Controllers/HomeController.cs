using Microsoft.AspNetCore.Mvc;

namespace _6TL.Areas.Admin.Controllers
{
	public class HomeController : Controller
	{
		[Area("Admin")]
		public IActionResult Index()
		{
			return View();
		}
	}
}
