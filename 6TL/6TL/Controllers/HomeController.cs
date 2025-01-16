using _6TL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace _6TL.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly Db6TLContext _context;
        private readonly IConfiguration _configuration;


        public HomeController(ILogger<HomeController> logger, Db6TLContext context, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
        }

		[HttpPost]
		[Route("Home/UpdateCartQuantity/{productId}")]
		public IActionResult UpdateCartQuantity([FromRoute] int productId, [FromBody] int quantity)
		{
			try
			{
				// Kiểm tra số lượng phải lớn hơn 0
				if (quantity <= 0)
				{
					return Json(new { success = false, message = "Số lượng phải lớn hơn 0." });
				}

				// Tìm sản phẩm trong giỏ hàng theo productId
				var cartItem = _context.Carts
					.FirstOrDefault(c => c.ProductId == productId);

				if (cartItem == null)
				{
					return Json(new { success = false, message = "Không tìm thấy sản phẩm trong giỏ hàng." });
				}

				// Kiểm tra số lượng tồn kho từ bảng Product
				var productStock = _context.Products
					.Where(p => p.ProductId == productId)
					.Select(p => p.Quantity)
					.FirstOrDefault();

				// Kiểm tra nếu số lượng yêu cầu lớn hơn số lượng tồn kho
				if (quantity > productStock)
				{
					return Json(new { success = false, message = $"Chỉ còn {productStock} sản phẩm trong kho." });
				}

				// Cập nhật số lượng và tổng tiền của sản phẩm trong giỏ hàng
				cartItem.Quantity = quantity;
				cartItem.TotalPrice = cartItem.Price * quantity;
				cartItem.UpdatedAt = DateTime.Now;

				_context.SaveChanges();

				// Tính lại tổng tiền của giỏ hàng
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

		// Controller method for handling the checkout
		public async Task<IActionResult> TrangThanhToan(
			int? productId = null,
			string productName = null,
			string productImage = null,
			decimal? productPrice = null,
			string productColor = null,
			int quantity = 1,
			bool isBuyNow = false)
		{
			// Lấy CustomerId từ session
			int? customerId = HttpContext.Session.GetInt32("CustomerId");

			// Kiểm tra xem người dùng đã đăng nhập chưa
			if (customerId == null || customerId == 0)
			{
				// Nếu chưa đăng nhập, lưu thông báo và chuyển hướng về trang chủ
				TempData["ErrorMessage"] = "Vui lòng đăng nhập để tiếp tục thanh toán.";
				return RedirectToAction("Index", "Home");  // Điều hướng về trang chủ
			}


			// Logic xử lý "Mua Ngay" hoặc giỏ hàng...
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

			// Logic xử lý thanh toán từ giỏ hàng...
			var cartItems = await _context.Carts
				.Where(c => c.CustomerId == customerId.Value)  // Lọc giỏ hàng của khách hàng đã đăng nhập
				.Include(c => c.Product)
				.ToListAsync();

			if (!cartItems.Any())
			{
				return RedirectToAction("GioHang");
			}

			decimal subtotal = cartItems.Sum(item => item.TotalPrice ?? (item.Price * item.Quantity));
			decimal total = subtotal;

			// Add the cart items to the view for rendering
			ViewBag.CartItems = cartItems;
			ViewBag.Subtotal = subtotal;
			ViewBag.Total = total;

			return View("TrangThanhToan");
		}

		[HttpPost]
		[Route("api/checkout/cod")]
		public IActionResult CheckoutCOD([FromBody] CartData cartData)
		{
			// Kiểm tra nếu khách hàng chưa đăng nhập
			int? customerId = HttpContext.Session.GetInt32("CustomerId");
			if (customerId == null || customerId == 0)
			{
				return Json(new { success = false, message = "Vui lòng đăng nhập để tiếp tục thanh toán." });
			}

			// Kiểm tra thông tin giao hàng từ cartData
			if (string.IsNullOrEmpty(cartData.customerName))
			{
				return Json(new { success = false, message = "Tên khách hàng không được để trống." });
			}

			if (string.IsNullOrEmpty(cartData.customerPhone) || !Regex.IsMatch(cartData.customerPhone, @"^\d{10}$"))
			{
				return Json(new { success = false, message = "Số điện thoại không hợp lệ. Vui lòng nhập đúng số điện thoại." });
			}

			if (string.IsNullOrEmpty(cartData.customerEmail) || !Regex.IsMatch(cartData.customerEmail, @"^[^@]+@[^@]+\.[^@]+$"))
			{
				return Json(new { success = false, message = "Email không hợp lệ. Vui lòng nhập đúng email." });
			}

			if (string.IsNullOrEmpty(cartData.customerAddress))
			{
				return Json(new { success = false, message = "Địa chỉ giao hàng không được để trống." });
			}

			// Kiểm tra phương thức thanh toán
			if (string.IsNullOrEmpty(cartData.paymentMethod))
			{
				return Json(new { success = false, message = "Vui lòng chọn phương thức thanh toán." });
			}

			// Kiểm tra giỏ hàng của khách hàng
			var cartItems = _context.Carts.Where(c => c.CustomerId == customerId.Value).ToList();
			if (!cartItems.Any())
			{
				return Json(new { success = false, message = "Giỏ hàng không có sản phẩm." });
			}

			// Tính tổng số tiền từ giỏ hàng
			decimal totalAmount = cartItems.Sum(item => item.TotalPrice ?? (item.Price * item.Quantity));

			// Tạo đơn hàng
			var order = new Order
			{
				CustomerId = customerId.Value,
				CustomerName = cartData.customerName,
				PhoneNumber = cartData.customerPhone,
				Address = cartData.customerAddress,
				Email = cartData.customerEmail,
				OrderDate = DateTime.Now,
				OrderStatus = "Chờ xử lý",
				TotalAmount = totalAmount,
				PaymentMethod = cartData.paymentMethod, // Lưu phương thức thanh toán từ cartData
				CreatedAt = DateTime.Now,
				UpdatedAt = DateTime.Now
			};

			try
			{
				// Thêm đơn hàng vào cơ sở dữ liệu
				_context.Orders.Add(order);
				_context.SaveChanges();

				// Lưu chi tiết đơn hàng
				foreach (var item in cartItems)
				{
					var orderDetail = new OrderDetail
					{
						OrderId = order.OrderId,
						ProductId = item.ProductId,
						ProductName = item.ProductName,
						Quantity = item.Quantity,
						UnitPrice = item.Price,
						TotalPrice = item.TotalPrice,
						TotalAmount = item.TotalPrice,
						Color = item.Color
					};
					_context.OrderDetails.Add(orderDetail);
				}
				_context.SaveChanges();

				// Xóa giỏ hàng của khách hàng
				_context.Carts.RemoveRange(cartItems);
				_context.SaveChanges();

				return Json(new { success = true, message = "Đặt hàng thành công!" });
			}
			catch (Exception ex)
			{
				// Xử lý khi có lỗi xảy ra
				return Json(new { success = false, message = "Đặt hàng thất bại: " + ex.Message });
			}
		}

		//Cứu chén
		[Route("TrangChucMung")]
		public IActionResult TrangChucMung()
		{
			return View();
		}


		// Dữ liệu nhận từ frontend
		public class CartData
		{
			public string customerName { get; set; }
			public string customerPhone { get; set; }
			public string customerEmail { get; set; }
			public string customerAddress { get; set; }
			public decimal totalAmount { get; set; }
			public List<CartItem> items { get; set; }
			public string paymentMethod { get; set; } // Thêm thuộc tính này
		}


		// Chi tiết từng sản phẩm trong giỏ hàng
		public class CartItem
		{
			public int productId { get; set; }
			public string productName { get; set; }
			public decimal price { get; set; }
			public int quantity { get; set; }
			public decimal totalPrice { get; set; }
			public string color { get; set; }
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
			int? customerId = HttpContext.Session.GetInt32("CustomerId");

			if (customerId == null)
			{
				// Return view with a flag indicating user is not logged in
				ViewBag.IsLoggedIn = false;
				ViewBag.CurrentPage = 1;
				ViewBag.TotalPages = 1;  // Ensure TotalPages is set to 1 if not logged in
				return View();
			}

			var totalItems = _context.Carts.Count(c => c.CustomerId == customerId);
			var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
			var cartItems = _context.Carts
				.Where(c => c.CustomerId == customerId)
				.Include(c => c.Product)
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToList();

			ViewBag.IsLoggedIn = true;
			ViewBag.CurrentPage = page;
			ViewBag.TotalPages = totalPages;  // Ensure TotalPages is set

			return View(cartItems);
		}



		[HttpDelete]
		[Route("Home/remove/{id}")]
		public IActionResult RemoveFromCart(int id)
		{
			// Kiểm tra người dùng đã đăng nhập hay chưa
			int? customerId = HttpContext.Session.GetInt32("CustomerId"); // Lấy từ session

			if (customerId == null)
			{
				// Nếu chưa đăng nhập, trả về lỗi
				return Json(new { success = false, message = "Vui lòng đăng nhập để thao tác." });
			}

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
			// Kiểm tra người dùng đã đăng nhập hay chưa
			int? customerId = HttpContext.Session.GetInt32("CustomerId"); // Lấy từ session

			if (customerId == null)
			{
				// Nếu chưa đăng nhập, trả về lỗi
				return Json(new { success = false, message = "Vui lòng đăng nhập để thao tác." });
			}

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
			// Kiểm tra người dùng đã đăng nhập hay chưa
			int? customerId = HttpContext.Session.GetInt32("CustomerId"); // Lấy từ session

			if (customerId == null)
			{
				// Nếu chưa đăng nhập, trả về 0
				return Json(new { totalQuantity = 0 });
			}

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
        public ActionResult TinTuc(int? pageNumber)
        {
            // Kích thước trang (số lượng bài viết trên mỗi trang)
            int pageSize = 6;

            // Số trang hiện tại, nếu không có thì mặc định là 1
            int page = pageNumber ?? 1;

            // Lấy tất cả các bài viết tin tức từ database
            var allNews = _context.Blogs.ToList();

            // Tính toán các bài viết cần hiển thị cho trang hiện tại
            var pageNews = allNews.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            // Truyền dữ liệu vào ViewBag
            ViewBag.News = pageNews;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(allNews.Count / (double)pageSize);

         /*   // Truyền dữ liệu vào ViewBag
            ViewBag.News = allNews;*/

            // Trả về view
            return View();
        }
		[HttpPost("register")]
		[Route("api/customer")]
		public async Task<IActionResult> DangKy(Customer model)
		{
			// Kiểm tra nếu đã đăng nhập, không cho phép đăng ký
			if (HttpContext.Session.GetString("CustomerId") != null)
			{
				return Json(new { success = false, message = "Bạn đã đăng nhập. Vui lòng đăng xuất trước khi đăng ký tài khoản mới." });
			}

			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var existingUser = _context.Customers.FirstOrDefault(u => u.Email == model.Email);
			if (existingUser != null)
			{
				ModelState.AddModelError("Email", "Email đã tồn tại. Vui lòng sử dụng email khác.");
				return View(model);
			}

			var tokenEmail = Guid.NewGuid().ToString();
			var user = new Customer
			{
				FullName = model.FullName,
				PhoneNumber = model.PhoneNumber,
				Email = model.Email,
				Address = model.Address,
				Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
				EmailConfirmationToken = tokenEmail,
				IsEmailConfirmed = false,
				RoleId = 2,
				Gender = model.Gender,
				DateOfBirth = model.DateOfBirth
			};

			try
			{
				_context.Customers.Add(user);
				await _context.SaveChangesAsync();

				var confirmationUrl = Url.Action("ConfirmEmail", "Home", new { token = tokenEmail }, Request.Scheme);
				await SendConfirmationEmail(model.Email, confirmationUrl);

				return View("DangKy", model);
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", $"Đã xảy ra lỗi: {ex.Message}");
				return View(model);
			}
		}



		private async Task SendConfirmationEmail(string email, string confirmationUrl)
		{
			//cài đặt môi trường kết nối gmail
			var smtpClient = new SmtpClient("smtp.gmail.com")
			{
				Port = 587,
				Credentials = new NetworkCredential("0306211392@caothang.edu.vn", "yybh tvcc zypk vjxh"),
				EnableSsl = true,
			};

			//cài đặt tin nhắn muốn gửi cho gmail người dùng nhập vào
			var mailMessage = new MailMessage
			{
				From = new MailAddress("0306211392@caothang.edu.vn"),
				Subject = "6TL FURNITURE CONFIRMATION",
				Body = $"<h1>CẢM ƠN BẠN ĐÃ ĐĂNG KÝ VÀ CHÚC MỪNG BẠN ĐÃ TRỞ THÀNH THÀNH VIÊN NHỎ TRONG GIA ĐÌNH 6TL</h1><p>Vui lòng nhấn vào liên kết sau để xác nhận:</p><a href='{confirmationUrl}'>Xác nhận email</a>",
				IsBodyHtml = true,
			};
			mailMessage.To.Add(email);

			await smtpClient.SendMailAsync(mailMessage);
		}

		[HttpGet]
		public async Task<IActionResult> ConfirmEmail(string token)
		{
			//kiểm tra token có hợp lệ hay không
			//nếu người dùng nhấn xác nhận gmail thêm lần nữa
			var customer = _context.Customers.FirstOrDefault(c => c.EmailConfirmationToken == token);
			//token không giống nhau
			if (customer == null)
			{
				return BadRequest("Token không hợp lệ.");
			}

			//cập nhật trạng thái xác nhận
			customer.IsEmailConfirmed = true;
			customer.EmailConfirmationToken = null;

			_context.Customers.Update(customer);
			await _context.SaveChangesAsync();

			ViewBag.Message = "Email đã được xác nhận. Bạn có thể đăng nhập ngay bây giờ!";
			return RedirectToAction("Index");
		}
		[HttpPost]
		public IActionResult DangNhap([FromBody] LoginModel model)
		{
			if (!ModelState.IsValid)
			{
				return Json(new { success = false, message = "Thông tin không hợp lệ." });
			}

			var customer = _context.Customers.FirstOrDefault(c => c.Email == model.Email);
			if (customer == null || !BCrypt.Net.BCrypt.Verify(model.Password, customer.Password) || customer.IsEmailConfirmed != true)
			{
				return Json(new { success = false, message = "Tài khoản chưa được xác nhận hoặc thông tin không chính xác." });
			}

			// Lưu thông tin khách hàng vào session
			HttpContext.Session.SetInt32("CustomerId", customer.CustomerId);
			HttpContext.Session.SetString("CustomerFullName", customer.FullName);

			// tạo token JWT nếu vẫn cần thiết (dùng cho API khác chẳng hạn)
			var token = GenerateJwtToken(customer);

			// trả về thông tin khách hàng và token
			return Json(new
			{
				success = true,
				token = token,
				customer = new
				{
					customer.CustomerId,
					customer.FullName,
					customer.Email,
					customer.PhoneNumber,
					customer.Address,
					customer.RoleId
				}
			});
		}



		//hàm tạo token
		private string GenerateJwtToken(Customer customer)
		{
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));//tạo một key
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var claims = new[]
			{
				new Claim(ClaimTypes.Name, customer.FullName),
				new Claim(ClaimTypes.Email, customer.Email),
				new Claim("CustomerId", customer.CustomerId.ToString())
			};

			var token = new JwtSecurityToken(
				issuer: _configuration["Jwt:Issuer"],
				audience: _configuration["Jwt:Audience"],
				claims: claims,
				expires: DateTime.Now.AddHours(2),
				signingCredentials: creds);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		public async Task<IActionResult> QuenMatKhau(ForgotPassword model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}
			var customer = _context.Customers.FirstOrDefault(c => c.Email == model.Email);
			if (customer == null)
			{
				ModelState.AddModelError("Email", "Email chưa được đăng kí.");
				return View(model);
			}
			var tokenEmail = Guid.NewGuid().ToString();
			customer.PasswordResetToken = tokenEmail;

			// Tính thời gian hết hạn cho token (10 phút, UTC+7)
			var currentDate = DateTime.UtcNow.AddHours(7); // Chuyển sang múi giờ UTC+7
			customer.PasswordResetExpires = currentDate.AddMinutes(10);

			try
			{
				// Lưu thay đổi vào cơ sở dữ liệu
				_context.Customers.Update(customer);
				await _context.SaveChangesAsync();

				var url = Url.Action("ThayDoiMatKhau", "Home", new { token = tokenEmail }, Request.Scheme);
				await SendPasswordEmail(model.Email, url);

				return RedirectToAction("Index");
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", $"Đã xảy ra lỗi: {ex.Message}");
				return View(model);
			}
		}
		private async Task SendPasswordEmail(string email, string url)
		{
			//cài đặt môi trường kết nối gmail
			var smtpClient = new SmtpClient("smtp.gmail.com")
			{
				Port = 587,
				Credentials = new NetworkCredential("0306211392@caothang.edu.vn", "yybh tvcc zypk vjxh"),
				EnableSsl = true,
			};

			//cài đặt tin nhắn muốn gửi cho gmail người dùng nhập vào
			var mailMessage = new MailMessage
			{
				From = new MailAddress("0306211392@caothang.edu.vn"),
				Subject = "Thay đổi mật khẩu",
				Body = $"Chào, Vui lòng theo dõi liên kết này để đặt lại mật khẩu của bạn. Liên kết này có giá trị đến 10 phút kể từ bây giờ. </p><a href='{url}'>Liên kết ở đây</a>",
				IsBodyHtml = true,
			};
			mailMessage.To.Add(email);

			await smtpClient.SendMailAsync(mailMessage);
		}
		public async Task<IActionResult> ThayDoiMatKhau(string token, ResetPassword model)
		{
			if (string.IsNullOrEmpty(token))
			{
				return RedirectToAction("Index", "Home"); // Nếu token rỗng, chuyển hướng về trang chủ
			}
			if (string.IsNullOrEmpty(token))
			{
				ModelState.AddModelError("", "Token không hợp lệ.");
				return View(model);
			}
			if (!ModelState.IsValid)
			{
				return View(model);
			}
			// Lấy thời gian hiện tại theo UTC+7
			DateTime currentDate = DateTime.UtcNow.AddHours(7);

			var customer = _context.Customers.FirstOrDefault(c => c.PasswordResetToken == token && c.PasswordResetExpires > currentDate);
			if (customer == null)
			{
				return Json(new { success = false, token = token, message = "Token đã hết hạn, Vui lòng thử lại sau." });
			}

			// Thiết lập lại mật khẩu và xoá thông tin đặt lại mật khẩu
			customer.Password = BCrypt.Net.BCrypt.HashPassword(model.Password); // Sử dụng phương thức hash mật khẩu
			customer.PasswordResetToken = null;
			customer.PasswordResetExpires = null;

			// Lưu thay đổi vào cơ sở dữ liệu
			_context.Customers.Update(customer);
			await _context.SaveChangesAsync();

			return RedirectToAction("Index");
		}

		public IActionResult DangXuat()
		{
			// Xóa toàn bộ dữ liệu trong Session
			HttpContext.Session.Clear();

			return Json(new { success = true, message = "Bạn đã đăng xuất thành công." });
		}

		public IActionResult ChinhSach()
		{
			return View();
		}
		public IActionResult DanhMucSanPham()
		{
			return View();
		}
        public IActionResult ChiTietTinTuc(int id)
        {
            var blog = _context.Blogs.FirstOrDefault(b => b.BlogId == id);
            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
