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
        public IActionResult ThemBlog()
        {
            return View();
        }
        public IActionResult QuanLyBlog()
        {
            return View();
        }
		
		public IActionResult GioiThieu()
		{
			return View();
		}
        public IActionResult QuanLyDanhMuc() 
        {
            return View();
        }
        public IActionResult QuanLyDonHang()
        {
            return View();
        }
        public IActionResult QuanLyChiTietDonHang()
        {
            return View();
        }
        public IActionResult QuanLyAdmin()
        {
            return View();
        }
        public IActionResult QuanLyUser()
        {
            return View();
        }
        public IActionResult QuanLyLienHe()
        {
            return View();
        }
        public IActionResult QuanLyDanhGia()
        {
            return View();
        }
    }
}
