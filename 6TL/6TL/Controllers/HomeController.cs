using _6TL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;

namespace _6TL.Controllers
{
	public class HomeController : Controller

	{
		private readonly Db6TLContext _dbContext;

		// Inject DbContext thông qua constructor
		public HomeController(Db6TLContext dbContext)
		{
			_dbContext = dbContext;
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
		public IActionResult LienHe()
		{
			return View();
		}

		[HttpPost]
		public IActionResult LienHe(Contact model)
		{
			if (ModelState.IsValid)
			{
				model.CreatedDate = DateTime.Now;

				// Lưu dữ liệu vào cơ sở dữ liệu
				_dbContext.Contacts.Add(model);
				_dbContext.SaveChanges();

				TempData["SuccessMessage"] = "Thông tin liên hệ đã được gửi thành công!";
				return RedirectToAction("LienHe");
			}

			TempData["ErrorMessage"] = "Vui lòng kiểm tra lại thông tin!";
			return View(model);
		}
		public IActionResult GioHang()
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
		public IActionResult ChiTietTinTuc()
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
