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

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(p => p.ProductName.Contains(searchString));
                ViewBag.SearchString = searchString;
            }

            var totalProducts = query.Count();
            var products = query
                .OrderBy(p => p.ProductId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalProducts / pageSize);
            ViewBag.BaseUrl = Url.Action("QuanLySanPham", "Products");
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

			// Trả về view với sản phẩm đã được lấy từ cơ sở dữ liệu
			return View(product);
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult SuaSanPham(int id, Product product, IFormFile? imageFile)
		{
			if (id != product.ProductId)
			{
				return BadRequest("ID mismatch");
			}

			try
			{
				// Lấy sản phẩm hiện tại từ cơ sở dữ liệu
				var existingProduct = _context.Products.Find(product.ProductId);
				if (existingProduct == null)
				{
					return NotFound(); // Nếu sản phẩm không tồn tại
				}

				// Cập nhật thông tin cơ bản
				existingProduct.ProductName = product.ProductName;
				existingProduct.Price = product.Price;
				existingProduct.Material = product.Material;
				existingProduct.ProductDescription = product.ProductDescription;
				existingProduct.CategoryId = product.CategoryId;
				existingProduct.SupplierId = product.SupplierId;
				existingProduct.Color = product.Color; // Cập nhật màu sắc từ cột Color
				existingProduct.Quantity = product.Quantity; // Cập nhật số lượng từ cột Quantity

				// Xử lý hình ảnh nếu có upload mới
				if (imageFile != null && imageFile.Length > 0)
				{
					string directoryPath = Path.Combine("wwwroot", "images", "products");
					if (!Directory.Exists(directoryPath))
					{
						Directory.CreateDirectory(directoryPath);
					}

					string fileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
					string filePath = Path.Combine(directoryPath, fileName);

					// Lưu file ảnh
					using (var fileStream = new FileStream(filePath, FileMode.Create))
					{
						imageFile.CopyTo(fileStream);
					}

					// Xóa ảnh cũ nếu có
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
			catch (Exception ex)
			{
				Console.WriteLine($"Error: {ex.Message}");
				TempData["ErrorMessage"] = "Có lỗi xảy ra trong quá trình cập nhật sản phẩm. Vui lòng thử lại sau.";
				return View(product); // Trả về view với dữ liệu hiện tại để hiển thị lỗi
			}
		}

		[Route("Admin/Products/QuanLySanPham/{productId}")]
		[HttpPost]
		public IActionResult XoaSanPham(int productId)
		{
			try
			{
				// Lấy sản phẩm theo ProductId
				var product = _context.Products.FirstOrDefault(p => p.ProductId == productId);

				if (product != null)
				{
					// Xóa sản phẩm
					_context.Products.Remove(product);

					// Lưu thay đổi vào cơ sở dữ liệu
					_context.SaveChanges();

					return Json(new { success = true, message = "Xóa sản phẩm thành công!" });
				}

				return Json(new { success = false, message = "Sản phẩm không tồn tại!" });
			}
			catch (Exception ex)
			{
				// Xử lý lỗi và trả về phản hồi JSON
				Console.WriteLine($"Error: {ex.Message}");
				return Json(new { success = false, message = "Đã xảy ra lỗi khi xóa sản phẩm. Vui lòng thử lại!" });
			}
		}





	}
}
