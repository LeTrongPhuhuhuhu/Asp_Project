using _6TL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _6TL.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        
        private Db6TLContext _context;

        public ProductsController(Db6TLContext context) {
            _context = context;
        }

        public IActionResult QuanLySanPham()
        {
            return View();
        }

        public IActionResult ThemSanPham(Product product)
        {
            if (ModelState.IsValid)
            {
                

                // Thêm sản phẩm vào cơ sở dữ liệu
                _context.Products.Add(product);
                _context.SaveChanges();

                // Chuyển hướng về trang quản lý sản phẩm
                return RedirectToAction("QuanLySanPham");
            }

            // Nếu có lỗi, trả lại view thêm sản phẩm
            return View(product);
        }
    

        public IActionResult SuaSanPham()
        {
            return View();
        }
    }
}
