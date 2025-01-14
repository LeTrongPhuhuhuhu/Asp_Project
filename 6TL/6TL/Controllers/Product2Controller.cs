using _6TL.Models;
using Microsoft.AspNetCore.Mvc;

namespace _6TL.Controllers
{
    public class Product2Controller : Controller
    {
        private readonly Db6tlContext _context;

        public Product2Controller(Db6tlContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult SanPham()
        {
            var products = _context.Products.ToList();// Lấy danh sách sản phẩm từ database
            return View(products); 
        }
    }
}
