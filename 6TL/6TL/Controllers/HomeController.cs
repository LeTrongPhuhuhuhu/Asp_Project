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

				// Sử dụng customerId = 1 tạm thời
				int customerId = 1;

				var cartItem = _context.Carts
					.FirstOrDefault(c => c.ProductId == productId && c.CustomerId == customerId);

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
				var subtotal = _context.Carts
					.Where(c => c.CustomerId == customerId)
					.Sum(c => c.TotalPrice) ?? 0;

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
	bool isBuyNow = false)
		{
			// Kiểm tra xem người dùng đã đăng nhập chưa
			int customerId = 1; // Tạm thời là 1, sau này sẽ lấy từ session hoặc token người dùng đã đăng nhập
			if (customerId == 0)  // Giả sử nếu không có customerId, người dùng chưa đăng nhập
			{
				return RedirectToAction("Login", "Account");  // Chuyển hướng đến trang đăng nhập
			}

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
			var cartItems = await _context.Carts
				.Where(c => c.CustomerId == customerId)  // Lọc giỏ hàng của khách hàng đã đăng nhập
				.Include(c => c.Product)
				.ToListAsync();

			if (!cartItems.Any())
			{
				return RedirectToAction("GioHang");
			}

			// Tính toán và trả về View cho Thanh Toán
			decimal subtotal = cartItems.Sum(item => item.TotalPrice ?? (item.Price * item.Quantity));

			decimal total = subtotal;

			ViewBag.CartItems = cartItems;
			ViewBag.Subtotal = subtotal;
			
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


		public IActionResult GioHang(int page = 1, int pageSize = 6)
		{
			// Giả sử bạn đã có logic để lấy CustomerId từ session hoặc thông tin đăng nhập
			int customerId = 1; // Tạm thời là 1, sau này sẽ lấy từ session hoặc token người dùng đã đăng nhập

			// Lấy tổng số sản phẩm trong giỏ hàng của khách hàng
			var totalItems = _context.Carts.Count(c => c.CustomerId == customerId);

			// Tính tổng số trang
			var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

			// Lấy danh sách sản phẩm phân trang của khách hàng
			var cartItems = _context.Carts
				.Where(c => c.CustomerId == customerId)
				.Include(c => c.Product)
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToList();

			// Truyền dữ liệu cần thiết sang View
			ViewBag.CurrentPage = page;
			ViewBag.TotalPages = totalPages;

			return View(cartItems);
		}

		[HttpDelete]
		[Route("Home/remove/{id}")]
		public IActionResult RemoveFromCart(int id)
		{
			// Giả sử bạn đã có logic để lấy CustomerId từ session hoặc thông tin đăng nhập
			int customerId = 1; // Tạm thời là 1, sau này sẽ lấy từ session hoặc token người dùng đã đăng nhập

			var cartItem = _context.Carts
				.FirstOrDefault(item => item.ProductId == id && item.CustomerId == customerId);

			if (cartItem != null)
			{
				_context.Carts.Remove(cartItem);
				_context.SaveChanges();

				// Kiểm tra nếu giỏ hàng trống
				if (!_context.Carts.Any(c => c.CustomerId == customerId))
				{
					// Trả về kết quả thành công với thông báo giỏ hàng trống
					return Json(new { success = true, emptyCart = true });
				}

				return Json(new { success = true });
			}

			return Json(new { success = false, message = "Sản phẩm không tồn tại trong giỏ hàng." });
		}



		[HttpPost]
		public IActionResult ClearCart()
		{
			// Giả sử bạn đã có logic để lấy CustomerId từ session hoặc thông tin đăng nhập
			int customerId = 1; // Tạm thời là 1, sau này sẽ lấy từ session hoặc token người dùng đã đăng nhập

			// Xóa tất cả các sản phẩm trong giỏ hàng của khách hàng
			var cartItems = _context.Carts.Where(c => c.CustomerId == customerId).ToList();

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
			// Giả sử bạn đã có logic để lấy CustomerId từ session hoặc thông tin đăng nhập
			int customerId = 1; // Tạm thời là 1, sau này sẽ lấy từ session hoặc token người dùng đã đăng nhập

			// Lấy tổng số lượng sản phẩm trong giỏ hàng của khách hàng
			var totalQuantity = _context.Carts
				.Where(c => c.CustomerId == customerId)
				.Sum(c => c.Quantity);

			return Json(new { totalQuantity });
		}



		public IActionResult Index()
		{
			using (var context = new Db6TLContext())
			{
				// Lọc sản phẩm nổi bật với `Rating >= 4`
				var featuredProducts = context.Products
					.Where(p => p.Rating >= 4) // Tiêu chí lọc: Rating >= 4
					.ToList();

				return View(featuredProducts); // Truyền danh sách sản phẩm nổi bật vào View
			}
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
