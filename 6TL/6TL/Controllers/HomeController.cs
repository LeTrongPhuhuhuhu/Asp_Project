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

		///Khó nhé
		[HttpPost]
		[Route("Home/UpdateCartQuantity/{productId}")]
		public IActionResult UpdateCartQuantity([FromRoute] int productId, [FromBody] int quantity)
		{
			try
			{
				if (quantity <= 0)
				{
					return Json(new { success = false, message = "Số lượng phải lớn hơn 0." });
				}

				var cartItem = _context.Carts
					.FirstOrDefault(c => c.ProductId == productId);

				if (cartItem == null)
				{
					return Json(new { success = false, message = "Không tìm thấy sản phẩm trong giỏ hàng." });
				}

				// Kiểm tra số lượng tồn kho
				var productStock = _context.ProductColors
					.Where(pc => pc.ProductId == productId && pc.ColorId ==
						_context.Colors.FirstOrDefault(c => c.ColorCode == cartItem.Color).ColorId)
					.Select(pc => pc.Quantity)
					.FirstOrDefault();

				if (quantity > productStock)
				{
					return Json(new { success = false, message = $"Chỉ còn {productStock} sản phẩm trong kho." });
				}

				// Cập nhật số lượng và tổng tiền
				cartItem.Quantity = quantity;
				cartItem.TotalPrice = cartItem.Price * quantity;
				cartItem.UpdatedAt = DateTime.Now;

				_context.SaveChanges();

				// Tính lại tổng cộng
				var subtotal = _context.Carts.Sum(c => c.TotalPrice) ?? 0;

				return Json(new
				{
					success = true,
					subtotal = subtotal,
					itemTotal = cartItem.TotalPrice,
					message = "Cập nhật số lượng thành công!"
				});
			}
			catch (Exception ex)
			{
				return Json(new { success = false, message = "Đã xảy ra lỗi khi cập nhật giỏ hàng." });
			}
		}
		// Trang thanh toán với thông tin sản phẩm

		// Action hiển thị trang thanh toán


		public async Task<IActionResult> TrangThanhToan(
	int? productId = null,
	string productName = null,
	string productImage = null,
	decimal? productPrice = null,
	string productColor = null,
	int quantity = 1,
	bool isBuyNow = false) // Nhận isBuyNow
		{
			if (isBuyNow)
			{
				// Logic khi nhấn "Mua Ngay"
				ViewBag.ProductId = productId ?? 0;
				ViewBag.ProductName = productName ?? "Unknown Product";
				ViewBag.ProductImage = productImage ?? "default.jpg";
				ViewBag.ProductPrice = (productPrice.HasValue ? productPrice.Value.ToString("N0") : "0") + " VNĐ";
				ViewBag.ProductColor = productColor ?? "No Color";
				ViewBag.Quantity = "x" + quantity;

				return View("TrangThanhToan"); // Trả về giao diện Mua Ngay
			}

			// Logic cho nút Thanh Toán từ giỏ hàng
			var cartItems = await _context.Carts.Include(c => c.Product).ToListAsync();
			if (!cartItems.Any())
			{
				return RedirectToAction("GioHang");
			}

			// Tính toán và trả về View cho Thanh Toán
			decimal subtotal = cartItems.Sum(item => item.TotalPrice ?? (item.Price * item.Quantity));
			decimal shippingFee = 50000; // Phí ship cố định
			decimal total = subtotal + shippingFee;

			ViewBag.CartItems = cartItems;
			ViewBag.Subtotal = subtotal;
			ViewBag.ShippingFee = shippingFee;
			ViewBag.Total = total;

			return View("TrangThanhToan");
		}



		//public ActionResult TrangThanhToan(int productId, string productName, string productImage, decimal productPrice, string productColor, int quantity)
		//{
		//	ViewBag.ProductId = productId;
		//	ViewBag.ProductName = productName;
		//	ViewBag.ProductImage = productImage;
		//	ViewBag.ProductPrice = productPrice.ToString("N0") + " VNĐ";
		//	ViewBag.ProductColor = productColor;
		//	ViewBag.Quantity = "x" + quantity;

		//	return View();
		//}


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
		[HttpGet]
		public IActionResult GetCartQuantity()
		{
			// Giả sử bạn có bảng Cart với cột Quantity
			var totalQuantity = _context.Carts.Sum(c => c.Quantity);
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
