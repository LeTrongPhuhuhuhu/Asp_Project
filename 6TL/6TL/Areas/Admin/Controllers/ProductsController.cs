using _6TL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _6TL.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {


        private Db6tlContext _context;

        public ProductsController(Db6tlContext context) {
            _context = context;
        }

        // Action để hiển thị danh sách sản phẩm
        public IActionResult QuanLySanPham()
        {
            var products = _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductColors)
                    .ThenInclude(pc => pc.Color)
                .ToList();

            return View(products);
        }




        [HttpGet]
        public IActionResult ThemSanPham()
        {
            // Lấy danh sách nhà cung cấp và danh mục từ cơ sở dữ liệu và gán cho ViewBag
            ViewBag.Suppliers = _context.Suppliers.ToList();
            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ThemSanPham(Product product, IFormFile? imageFile)
        {
            // Kiểm tra xem ModelState có hợp lệ không
            if (ModelState.IsValid)
            {
                // Kiểm tra nếu có file hình ảnh được chọn
                if (imageFile != null && imageFile.Length > 0)
                {
                    // Tạo tên file duy nhất cho hình ảnh
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products", uniqueFileName);

                    // Lưu hình ảnh vào thư mục
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        imageFile.CopyTo(stream);
                    }

                    // Cập nhật đường dẫn hình ảnh vào đối tượng product
                    product.Image = "/images/products/" + uniqueFileName;
                }

                // Cập nhật thời gian tạo sản phẩm
                product.CreatedAt = DateTime.Now;

                // Thêm sản phẩm vào cơ sở dữ liệu
                _context.Products.Add(product);
                _context.SaveChanges();

                // Thông báo thành công
                TempData["SuccessMessage"] = "Thêm sản phẩm thành công!";
                return RedirectToAction("QuanLySanPham");
            }
            TempData["SuccessMessage"] = "Thêm sản phẩm thất bại ";
            // Nếu có lỗi, tải lại danh mục và nhà cung cấp
            ViewBag.Suppliers = _context.Suppliers.ToList();
            ViewBag.Categories = _context.Categories.ToList();
            return View(product);
        }



        public IActionResult SuaSanPham()
        {
            return View();
        }
    }
}
