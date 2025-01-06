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


		public IActionResult GioHang()
		{
			var cartItems = _context.Carts
				.Include(c => c.Product)
				.ToList();
			return View(cartItems ?? new List<Cart>());
		}

		[HttpDelete]
		[Route("Home/remove/{id}")]
		public IActionResult RemoveFromCart(int id)
		{
			var cartItem = _context.Carts.FirstOrDefault(item => item.ProductId == id);
			if (cartItem != null)
			{
				_context.Carts.Remove(cartItem);
				_context.SaveChanges();

				// Kiểm tra nếu giỏ hàng trống
				if (!_context.Carts.Any())
				{
					// Trả về kết quả thành công với thông báo giỏ hàng trống
					return Json(new { success = true, emptyCart = true });
				}

				return Json(new { success = true });
			}
			return Json(new { success = false, message = "Sản phẩm không tồn tại trong giỏ hàng." });
		}


		// Trong HomeController
		[HttpPost]
		public IActionResult ClearCart()
		{
			// Xóa tất cả các sản phẩm trong bảng Cart
			var cartItems = _context.Carts.ToList();

			if (cartItems.Any())
			{
				_context.Carts.RemoveRange(cartItems);
				_context.SaveChanges();
			}

			// Chuyển hướng đến trang giỏ hàng
			return RedirectToAction("GioHang"); // Giả sử view giỏ hàng của bạn tên là "Cart"
		}
		public int GetTotalCartQuantity() {
			return _context.Carts.Sum(c => c.Quantity); }
		public IActionResult GetCartQuantity() { 
			int totalQuantity = GetTotalCartQuantity();
			return Json(new { totalQuantity });
		}

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
   
        public IActionResult ChiTietSanPham() { return View(); }


		public IActionResult ViewProfile()
        {
            return View();
        }
		public IActionResult TinTuc()
		{
			// Lấy tất cả các bài viết tin tức từ database
			var allNews = _context.Blogs.ToList();

			// Truyền dữ liệu vào ViewBag
			ViewBag.News = allNews;

			// Trả về view
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
