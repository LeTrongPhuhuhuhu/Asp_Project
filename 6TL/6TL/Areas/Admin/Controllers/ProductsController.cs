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
            ViewBag.Colors = _context.Colors.ToList();
            ViewBag.Suppliers = _context.Suppliers.ToList();
            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ThemSanPham(Product product, IFormFile? imageFile, List<ProductColor> Colors)
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

                // Thêm sản phẩm
                product.CreatedAt = DateTime.Now;
                _context.Products.Add(product);
                _context.SaveChanges();

                // Thêm màu sắc và số lượng
                if (Colors != null && Colors.Count > 0)
                {
                    foreach (var productColor in Colors)
                    {
                        // Tìm màu sắc trong bảng Colors
                        var existingColor = _context.Colors.FirstOrDefault(c => c.ColorName == productColor.Color.ColorName);

                        if (existingColor != null)
                        {
                            // Nếu màu sắc đã tồn tại, kiểm tra liên kết trong bảng ProductColor
                            var existingProductColor = _context.ProductColors
                                .FirstOrDefault(pc => pc.ProductId == product.ProductId && pc.ColorId == existingColor.ColorId);

                            if (existingProductColor != null)
                            {
                                // Nếu liên kết đã tồn tại, cập nhật số lượng
                                existingProductColor.Quantity += productColor.Quantity;
                            }
                            else
                            {
                                // Nếu liên kết chưa tồn tại, thêm mới với số lượng
                                var newProductColor = new ProductColor
                                {
                                    ProductId = product.ProductId,
                                    ColorId = existingColor.ColorId,
                                    Quantity = productColor.Quantity
                                };
                                _context.ProductColors.Add(newProductColor);
                            }
                        }
                        else
                        {
                            // Nếu màu sắc chưa tồn tại, tạo mới trong bảng Colors
                            var newColor = new Color
                            {
                                ColorName = productColor.Color.ColorName
                            };
                            _context.Colors.Add(newColor);
                            _context.SaveChanges(); // Lưu để lấy ColorId

                            // Sau khi tạo mới, thêm liên kết vào bảng ProductColor với số lượng
                            var newProductColor = new ProductColor
                            {
                                ProductId = product.ProductId,
                                ColorId = newColor.ColorId,
                                Quantity = productColor.Quantity
                            };
                            _context.ProductColors.Add(newProductColor);
                        }
                    }

                    // Lưu tất cả thay đổi vào cơ sở dữ liệu
                    _context.SaveChanges();
                }

                TempData["SuccessMessage"] = "Thêm sản phẩm thành công!";
                return RedirectToAction("Index");
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





        public IActionResult SuaSanPham()
        {
            return View();
        }
    }
}
