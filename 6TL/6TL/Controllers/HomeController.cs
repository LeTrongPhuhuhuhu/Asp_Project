using _6TL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace _6TL.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly Db6TLContext _context;

	

		public HomeController(ILogger<HomeController> logger, Db6TLContext context)
		{
			_logger = logger;
			_context = context;
		}
		public IActionResult SanPham() { return View(); }
		// Trang thanh toán với thông tin sản phẩm
		
			// Action hiển thị trang thanh toán
			public ActionResult TrangThanhToan(int productId, string productName, string productImage, decimal productPrice, string productColor, int quantity)
			{
				ViewBag.ProductId = productId;
				ViewBag.ProductName = productName;
				ViewBag.ProductImage = productImage;
			ViewBag.ProductPrice = productPrice.ToString("N0") + " VNĐ";
			ViewBag.ProductColor = productColor;
				ViewBag.Quantity = "x"+quantity;

				return View();
			}
		


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
        public IActionResult GioHang()
        {
            return View();
        }
        public IActionResult ChiTietSanPham() { return View(); }


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
