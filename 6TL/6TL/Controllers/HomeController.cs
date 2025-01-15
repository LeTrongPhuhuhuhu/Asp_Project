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

		public IActionResult ChiTietSanPham() { return View(); }


		public IActionResult ViewProfile()
		{
			return View();
		}
        public IActionResult TinTuc(string? keyword, int pageNumber = 1, int pageSize = 6)
        {
            // Tìm kiếm tin tức theo từ khóa
            var query = _context.Blogs.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(b => b.TieuDe.Contains(keyword) || b.NoiDung.Contains(keyword));
                ViewBag.Keyword = keyword; // Gửi từ khóa về View để hiển thị lại
            }

            // Phân trang
            int totalRecords = query.Count();
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            var blogs = query
                .OrderByDescending(b => b.CreatedAt) // Sắp xếp mới nhất
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Gửi dữ liệu về View
            ViewBag.News = blogs;
            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = totalPages;

            return View();
        }
        public async Task<IActionResult> DangKy(Customer model)
        {
			if (!ModelState.IsValid)
			{
				return View(model);
			}

            // kiểm tra email có tồn tại chưa
            var existingUser = _context.Customers.FirstOrDefault(u => u.Email == model.Email);
            if (existingUser != null) // nếu rồi thì hiển thị lỗi
            {
                ModelState.AddModelError("Email", "Email đã tồn tại. Vui lòng sử dụng email khác.");
                return View(model);
            }
            //tạo một token
            var tokenEmail = Guid.NewGuid().ToString();
            //nếu chưa có email thì tạo người dùng mới
            var user = new Customer
            {
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                Address = model.Address,
                Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                EmailConfirmationToken = tokenEmail,
                IsEmailConfirmed = false, // gán xác nhận là false
                RoleId = 2 // quyền mặc định
            };

            try
            {
                _context.Customers.Add(user);
                _context.SaveChanges();// lưu nhưng ch đăng nhập được
                // gửi email xác nhận
                var confirmationUrl = Url.Action("ConfirmEmail", "Home", new { token = tokenEmail }, Request.Scheme);
                await SendConfirmationEmail(model.Email, confirmationUrl);

                // trả về thông báo thành công vào View
                var message = "Vui lòng kiểm tra email của bạn để xác nhận.";
                return View("DangKy", model);  // truyền lại model nếu cần và thông báo
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
                Credentials = new NetworkCredential("hoangan0365800154@gmail.com", "xdad fwoe zwhk mzdg"),
                EnableSsl = true,
            };

            //cài đặt tin nhắn muốn gửi cho gmail người dùng nhập vào
            var mailMessage = new MailMessage
            {
                From = new MailAddress("hoangan0365800154@gmail.com"),
                Subject = "Xác nhận email",
                Body = $"<h1>Xác nhận email của bạn</h1><p>Vui lòng nhấn vào liên kết sau để xác nhận:</p><a href='{confirmationUrl}'>Xác nhận email</a>",
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


            // tạo token JWT
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
            // trả về kết quả thành công
            return Json(new { success = true });
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
