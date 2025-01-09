using _6TL.Data;
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
				.Include(c => c.Product)  // Bao gồm thông tin sản phẩm
			
				.ToList();

			// Kiểm tra nếu giỏ hàng trống
			if (!cartItems.Any())
			{
				ViewBag.IsCartEmpty = true;
			}
			else
			{
				ViewBag.IsCartEmpty = false;
			}

			return View(cartItems);
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
		public IActionResult TinTuc(int? pageNumber)
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
