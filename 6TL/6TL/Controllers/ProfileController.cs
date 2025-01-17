using _6TL.Models;
using Microsoft.AspNetCore.Mvc;

namespace _6TL.Controllers
{
    public class ProfileController : Controller
    {
        private readonly Db6TLContext _context;
        public ProfileController(Db6TLContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult ViewProfile()
        {
            try
            {
                // Lấy CustomerId từ Session
                var customerId = HttpContext.Session.GetInt32("CustomerId");
               
                if (customerId == null)
                {
                    ViewBag.ErrorMessage = "Vui lòng đăng nhập để xem hồ sơ.";
                    return View(); // Hiển thị thông báo trên trang hiện tại
                }

                // Lấy thông tin khách hàng từ cơ sở dữ liệu
                var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == customerId);
                if (customer == null)
                {
                    ViewBag.ErrorMessage = "Không thể tải thông tin hồ sơ. Vui lòng thử lại sau.";
                    return View(); // Hiển thị thông báo trên trang hiện tại
                }

                return View(customer); // Truyền thông tin khách hàng đến View
            }
            catch (Exception ex)
            {
                // Ghi log nếu cần
                Console.WriteLine($"Lỗi: {ex.Message}");
                ViewBag.ErrorMessage = "Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau.";
                return View(); // Hiển thị thông báo trên trang hiện tại
            }
        }

    }
}
