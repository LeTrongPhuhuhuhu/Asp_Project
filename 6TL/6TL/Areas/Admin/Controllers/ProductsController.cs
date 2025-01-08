using _6TL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace _6TL.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {


        private Db6TLContext _context;

        public ProductsController(Db6TLContext context)
        {
            _context = context;
        }

        // Action để hiển thị danh sách sản phẩm
        public IActionResult QuanLySanPham()
        {
            ViewBag.Categories = _context.Categories.ToList();
            // Truy vấn dữ liệu và trả về danh sách sản phẩm
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
        public IActionResult ThemSanPham(Product product, IFormFile? imageFile, List<ProductColor> Colors)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Suppliers = _context.Suppliers.Include(s => s.Products).ToList();

                ViewBag.Categories = _context.Categories.Include(s => s.Products).ToList();
                TempData["ErrorMessage"] = "Vui lòng kiểm tra lại thông tin sản phẩm.";
                return View(product);
            }

            try
            {
                // Tạo slug tự động từ tên sản phẩm
                product.Slug = GenerateSlug(product.ProductName);

                // Xử lý file ảnh
                if (imageFile != null && imageFile.Length > 0)
                {
                    string directoryPath = Path.Combine("wwwroot", "images", "products");
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath); // Tạo thư mục nếu chưa tồn tại
                    }

                    string fileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                    string filePath = Path.Combine(directoryPath, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        imageFile.CopyTo(fileStream);
                    }
                }

                // Thêm sản phẩm
                product.CreatedAt = DateTime.Now;
                _context.Products.Add(product);
                _context.SaveChanges();

                // Thêm màu sắc
                if (Colors != null && Colors.Count > 0)
                {
                    foreach (var color in Colors)
                    {
                        color.ProductId = product.ProductId;
                        _context.ProductColors.Add(color);
                    }
                    _context.SaveChanges();
                }

                TempData["SuccessMessage"] = "Thêm sản phẩm thành công!";
                return RedirectToAction("QuanLySanPham");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }

                return View(product);
            }
        }




        // Hàm tiện ích: Tạo slug từ tên sản phẩm
        private string GenerateSlug(string productName)
        {
            if (string.IsNullOrWhiteSpace(productName))
                return string.Empty;

            // Chuyển về chữ thường
            string slug = productName.ToLower();

            // Loại bỏ dấu tiếng Việt
            slug = RemoveVietnameseAccents(slug);

            // Thay khoảng trắng và các ký tự đặc biệt bằng dấu gạch ngang
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\s-]", ""); // Chỉ giữ lại chữ cái, số, khoảng trắng, và gạch ngang
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"\s+", "-").Trim(); // Thay khoảng trắng bằng gạch ngang
            return slug;
        }

        // Hàm tiện ích: Loại bỏ dấu tiếng Việt
        private string RemoveVietnameseAccents(string input)
        {
            string[] vietnameseChars = new string[]
            {
        "aáàạảãâấầậẩẫăắằặẳẵ", "oóòọỏõôốồộổỗơớờợởỡ",
        "eéèẹẻẽêếềệểễ", "uúùụủũưứừựửữ",
        "iíìịỉĩ", "yýỳỵỷỹ", "dđ",
        "AÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ", "OÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
        "EÉÈẸẺẼÊẾỀỆỂỄ", "UÚÙỤỦŨƯỨỪỰỬỮ",
        "IÍÌỊỈĨ", "YÝỲỴỶỸ", "DĐ"
            };

            string[] replacements = new string[]
            {
        "a", "o", "e", "u", "i", "y", "d",
        "a", "o", "e", "u", "i", "y", "d"
            };

            for (int i = 0; i < vietnameseChars.Length; i++)
            {
                foreach (char c in vietnameseChars[i])
                {
                    input = input.Replace(c.ToString(), replacements[i]);
                }
            }

            return input;
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }





        public IActionResult SuaSanPham()
        {
            return View();
        }
    }
}
