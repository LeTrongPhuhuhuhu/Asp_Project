using _6TL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _6TL.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly Db6TLContext _context;
        public HomeController(Db6TLContext context) {
            _context = context;
        }

        // Action Index - Hiển thị thông tin website
        [HttpGet]
        [Route("Admin/Home/Index")]
        public IActionResult Index()
        {
            var totalProduct = _context.Products.Count();
            var totalOrder = _context.Orders.Count();
            var totalRevenue = _context.Orders.Sum(od => od.TotalAmount);
            var websiteinfo = _context.WebsiteInfos.FirstOrDefault();

            ViewBag.TotalProducts = totalProduct;
            ViewBag.TotalOrders = totalOrder;
            ViewBag.TotalRevenue = totalRevenue;

            return View(websiteinfo);
        }

        // Action xử lý cập nhật thông tin website khi form gửi tới
        [HttpPost]
        [Route("Admin/Home/Index")]
        public IActionResult Index(WebsiteInfo model)
        {
            if (ModelState.IsValid)
            {
                var websiteInfo = _context.WebsiteInfos.FirstOrDefault();
                if (websiteInfo == null)
                {
                    TempData["Error"] = "Website information not found.";
                    return RedirectToAction("Index");
                }
                // Cập nhật thông tin website
                websiteInfo.Description = model.Description;
                websiteInfo.PhoneNumber = model.PhoneNumber;
                websiteInfo.Address = model.Address;
                websiteInfo.Email = model.Email;
                websiteInfo.FacebookUrl = model.FacebookUrl;
                websiteInfo.YouTubeUrl = model.YouTubeUrl;
                websiteInfo.TwitterUrl = model.TwitterUrl;
                websiteInfo.InstagramUrl = model.InstagramUrl;
                websiteInfo.LogoUrl = model.LogoUrl;

                _context.SaveChanges();
                TempData["Message"] = "Cập nhật thành công thông tin website!";
            }
            else
            {
                TempData["Error"] = "Có lỗi xảy ra! Vui lòng thử lại.";
            }
            var totalProduct = _context.Products.Count();
            var totalOrder = _context.Orders.Count();
            var totalRevenue = _context.Orders.Sum(od => od.TotalAmount);
            var websiteinfo = _context.WebsiteInfos.FirstOrDefault();

            // Cập nhật lại các thống kê vào ViewBag
            ViewBag.TotalProducts = totalProduct;
            ViewBag.TotalOrders = totalOrder;
            ViewBag.TotalRevenue = totalRevenue;

            return View(websiteinfo);
            
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
