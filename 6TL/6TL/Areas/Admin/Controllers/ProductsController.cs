using _6TL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Diagnostics.Metrics;
using System.Runtime.ConstrainedExecution;
using System;
using Microsoft.CodeAnalysis;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Web;

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
			// Lấy danh sách danh mục
			ViewBag.Categories = _context.Categories.ToList();

			// Truy vấn dữ liệu sản phẩm, chỉ lấy thông tin từ bảng Products và Category
			var products = _context.Products
				.Include(p => p.Category) // Chỉ include bảng Category
				.ToList();
            ViewBag.Categories = _context.Categories.ToList();
            return View(products);
		}

       // Tìm kiếm sản phẩm theo từ khóa
        [HttpGet]
        [Route("Admin/Products/QuanLySanPham")]
        public IActionResult TimKiemSanPham(int page = 1, int pageSize = 10, string searchString = "")
        {
            var query = _context.Products.AsQueryable();

            // Kiểm tra xem từ khóa có hợp lệ hay không
            if (!string.IsNullOrEmpty(searchString))
            {
                // Kiểm tra nếu từ khóa chỉ chứa dấu cách hoặc các ký tự đặc biệt
                if (string.IsNullOrWhiteSpace(searchString) || !searchString.All(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)))
                {
                    TempData["Message"] = "Vui lòng nhập từ khóa hợp lệ để tìm kiếm."; // Thông báo nếu từ khóa không hợp lệ
                    return View("QuanLySanPham", new List<Product>()); // Trả về view với danh sách sản phẩm rỗng
                }

                query = query.Where(p => p.ProductName.Contains(searchString));
                ViewBag.SearchString = searchString;
            }

            // Lấy danh sách sản phẩm theo trang
            var totalProducts = query.Count();
            var products = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            // Nếu không tìm thấy sản phẩm, hiển thị thông báo
            if (totalProducts == 0)
            {
                TempData["Message"] = "Không tìm thấy sản phẩm.";
            }

            // Phân trang
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalProducts / pageSize);
            ViewBag.BaseUrl = Url.Action("QuanLySanPham", "Products");
            ViewBag.Categories = _context.Categories.ToList();
            return View("QuanLySanPham", products);
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

                    product.Image = Path.Combine("images", "products", fileName); // Save image path to product
                }

                // Kiểm tra sản phẩm cùng màu đã tồn tại hay chưa
                var existingProduct = _context.Products
                    .FirstOrDefault(p => p.ProductName == product.ProductName && p.Color == product.Color);

                if (existingProduct != null)
                {
                    // Nếu sản phẩm đã tồn tại, chỉ cập nhật số lượng
                    existingProduct.Quantity += product.Quantity;
                    existingProduct.UpdatedAt = DateTime.Now;
                }
                else
                {
                    // Nếu sản phẩm chưa tồn tại, thêm mới sản phẩm
                    product.CreatedAt = DateTime.Now;
                    _context.Products.Add(product);
                }

                // Lưu thay đổi vào cơ sở dữ liệu
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Thêm sản phẩm thành công!";
                ViewBag.Categories = _context.Categories.ToList();
                return RedirectToAction("QuanLySanPham", "Products", new { area = "Admin" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }

                TempData["ErrorMessage"] = "Có lỗi xảy ra, vui lòng thử lại.";
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




        // GET action để hiển thị thông tin sản phẩm
        [Route("Admin/Products/SuaSanPham/{ProductId}")]
        [HttpGet]
        public IActionResult SuaSanPham(int ProductId)
        {
            // Lấy sản phẩm từ cơ sở dữ liệu dựa trên ProductId
            var product = _context.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .FirstOrDefault(p => p.ProductId == ProductId);

            if (product == null)
            {
                return NotFound(); // Nếu không tìm thấy sản phẩm
            }

            // Tạo danh sách cho các dropdown từ cơ sở dữ liệu
            ViewBag.Categories = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            ViewBag.Suppliers = new SelectList(_context.Suppliers, "SupplierId", "SupplierName", product.SupplierId);
            ViewBag.OldImage = product.Image; // Truyền ảnh cũ qua ViewBag
            // Trả về view với sản phẩm đã được lấy từ cơ sở dữ liệu
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SuaSanPham(Product product, IFormFile? imageFile)
        {
            // Lấy sản phẩm hiện tại từ cơ sở dữ liệu
            var existingProduct = _context.Products.FirstOrDefault(p => p.ProductId == product.ProductId);
            if (existingProduct == null)
            {
                return NotFound(); // Nếu sản phẩm không tồn tại
            }

            // Kiểm tra dữ liệu
            if (product.Price <= 0 || !decimal.TryParse(product.Price.ToString(), out _))
            {
                ModelState.AddModelError("Price", "Giá bán phải là số dương và có thể là số thập phân.");
                ViewBag.Categories = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
                ViewBag.Suppliers = new SelectList(_context.Suppliers, "SupplierId", "SupplierName", product.SupplierId);

                return View(product); // Trả về View với thông báo lỗi
            }

            if (product.CapitalAmount <= 0 || !decimal.TryParse(product.CapitalAmount.ToString(), out _))
            {
                ModelState.AddModelError("CapitalAmount", "Giá vốn phải là số dương và có thể là số thập phân.");
                ViewBag.Categories = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
                ViewBag.Suppliers = new SelectList(_context.Suppliers, "SupplierId", "SupplierName", product.SupplierId);

                return View(product); // Trả về View với thông báo lỗi
            }

            if (product.Quantity <= 0)
            {
                ModelState.AddModelError("Quantity", "Số lượng phải là số dương.");
                ViewBag.Categories = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
                ViewBag.Suppliers = new SelectList(_context.Suppliers, "SupplierId", "SupplierName", product.SupplierId);

                return View(product); // Trả về View với thông báo lỗi
            }

            if (product.CapitalAmount >= product.Price)
            {
                ModelState.AddModelError("CapitalAmount", "Giá vốn phải nhỏ hơn giá bán.");
                ViewBag.Categories = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
                ViewBag.Suppliers = new SelectList(_context.Suppliers, "SupplierId", "SupplierName", product.SupplierId);

                return View(product); // Trả về View với thông báo lỗi
            }

            if (imageFile != null)
            {
                // Kiểm tra định dạng và kích thước file
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var fileExtension = Path.GetExtension(imageFile.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("ImageFile", "Hình ảnh phải có định dạng JPG, PNG.");
                    ViewBag.Categories = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
                    ViewBag.Suppliers = new SelectList(_context.Suppliers, "SupplierId", "SupplierName", product.SupplierId);

                    return View(product); // Trả về View với thông báo lỗi
                }

                if (imageFile.Length > 5 * 1024 * 1024) // Giới hạn 5MB
                {
                    ModelState.AddModelError("ImageFile", "Hình ảnh không được vượt quá 5MB.");
                    ViewBag.Categories = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
                    ViewBag.Suppliers = new SelectList(_context.Suppliers, "SupplierId", "SupplierName", product.SupplierId);

                    return View(product); // Trả về View với thông báo lỗi

                }
            }
            // Cập nhật thông tin sản phẩm
            existingProduct.ProductName = product.ProductName;
            existingProduct.Price = product.Price;
            existingProduct.Material = product.Material;
            existingProduct.ProductDescription = product.ProductDescription;
            existingProduct.CategoryId = product.CategoryId;
            existingProduct.SupplierId = product.SupplierId;
            existingProduct.Color = product.Color;
            existingProduct.Quantity = product.Quantity;
            existingProduct.Slug = GenerateSlugEdit(product.ProductName);

            if (imageFile != null && imageFile.Length > 0)
            {
                string directoryPath = Path.Combine("wwwroot", "images", "products");
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                    string fileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                string filePath = Path.Combine(directoryPath, fileName);

                // Lưu file ảnh mới
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    imageFile.CopyTo(fileStream);
                }

                // Xóa ảnh cũ nếu tồn tại
                if (!string.IsNullOrEmpty(existingProduct.Image))
                {
                    string oldImagePath = Path.Combine("wwwroot", existingProduct.Image);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                // Gán đường dẫn ảnh mới
                existingProduct.Image = Path.Combine("images", "products", fileName);
            }

            // Cập nhật thời gian chỉnh sửa
            existingProduct.UpdatedAt = DateTime.Now;

            // Lưu thay đổi vào cơ sở dữ liệu
            _context.Products.Update(existingProduct);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Cập nhật sản phẩm thành công!";
            return RedirectToAction("QuanLySanPham");
        }


        private string GenerateSlugEdit(string productName)
        {
            // Thực hiện chuyển tên sản phẩm thành Slug: bỏ dấu, chuyển thành chữ thường, thay khoảng trắng bằng dấu gạch ngang
            return string.Concat(productName.ToLower()
                .Where(c => !char.IsWhiteSpace(c)) // Bỏ dấu cách
                .Select(c => char.IsLetterOrDigit(c) ? c : '-')) // Thay các ký tự không phải chữ cái hoặc số thành dấu gạch ngang
                .Trim('-'); // Loại bỏ dấu gạch ngang thừa ở đầu và cuối
        }





        [Route("Admin/Products/QuanLySanPham/{productId}")]
		[HttpPost]
        public IActionResult XoaSanPham(int productId)
        {
            
                // Kiểm tra sản phẩm có tồn tại không
                var product = _context.Products.FirstOrDefault(p => p.ProductId == productId);
                if (product == null)
                {
                    return Json(new { success = false, message = "Sản phẩm không tồn tại!" });
                }

                // Kiểm tra xem sản phẩm có liên kết với đơn hàng đang xử lý không
                bool hasOrders = _context.OrderDetails
                    .Any(od => od.ProductId == productId && _context.Orders
                        .Any(o => o.OrderId == od.OrderId && o.OrderStatus == "Đang xử lý"));

                if (hasOrders)
                {
                    // Nếu sản phẩm có liên kết với đơn hàng đang xử lý
                    TempData["Message"] = "Không thể xóa sản phẩm này vì nó đang liên kết với đơn hàng đang xử lý.";
                    return RedirectToAction("QuanLySanPham");
                }

                // Xóa sản phẩm nếu không có đơn hàng liên quan
                _context.Products.Remove(product);
                _context.SaveChanges();
            ViewBag.Categories = _context.Categories.ToList();
            // Trả về phản hồi thành công
            return Json(new { success = true, message = "Xóa sản phẩm thành công!" });
            }

        }






    
}
