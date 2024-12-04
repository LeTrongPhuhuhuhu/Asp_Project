using _6TL.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace _6TL.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;

		public HomeController(ILogger<HomeController> logger)
		{
			_logger = logger;
		}
		public IActionResult SanPham() { return View(); }

		public IActionResult SanPhamYeuThich() { return View(); }
		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}
        public IActionResult LichSuMuaHang()
        {
            return View();
        }
		public IActionResult GioiThieu()
		{
			return View();
		}
		public IActionResult ChiTietSanPham() { return View(); }

		public IActionResult TrangThanhToan() { return View(); }
		public IActionResult ViewProfile()
        {
            return View();
        }
		public IActionResult TinTuc()
		{
			return View();
		}
        public IActionResult DangKy()
        {
            return View();
        }
        public IActionResult DangNhap()
        {
            return View();
        }
		  public IActionResult ChinhSach()
        {
            return View();
        }
		public IActionResult DanhMucSanPham()
		{
			return View();
		}
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
