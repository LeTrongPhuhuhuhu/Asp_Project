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
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                return RedirectToAction("DangNhap"); // Chuyển hướng nếu chưa đăng nhập
            }

            var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == customerId);
            if (customer == null)
            {
                return RedirectToAction("DangNhap"); // Chuyển hướng nếu không tìm thấy khách hàng
            }

            return View(customer); // Truyền thông tin khách hàng đến view
        }
    }
}
