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
            //var username = User.Identity.Name;
            var username = "nguyenhoangthien120304@gmail.com"; // Giá trị giả để kiểm tra
            var customer = _context.Customers
                                    .FirstOrDefault(c => c.Email == username); // Thay đổi điều kiện theo trường định danh của bạn

            if (customer == null)
            {
                // Nếu không tìm thấy khách hàng, có thể chuyển hướng đến trang đăng nhập
                return RedirectToAction("DangNhap", "Home");
            }

            // Trả thông tin khách hàng vào View
            return View(customer);
        }
    }
}
