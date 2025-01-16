using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _6TL.Models; // Namespace của model
using System.Linq;

namespace _6TL.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DanhMucController : Controller
    {
        private readonly Db6tlContext _context;

        public DanhMucController(Db6tlContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách danh mục
        public IActionResult QuanLyDanhMuc(string searchQuery)
        {
            var categories = _context.Categories
                                     .Include(c => c.ParentCategory)
                                     .Where(c => string.IsNullOrEmpty(searchQuery) || c.CategoryName.Contains(searchQuery))
                                     .ToList();

            ViewBag.ParentCategories = _context.Categories
                                               .Where(c => c.ParentCategoryId == null)
                                               .ToList();

            ViewBag.SearchQuery = searchQuery;

            return View(categories);
        }
        // Hàm kiểm tra vòng lặp danh mục cha - con
        private bool HasCircularReference(int? parentId, int categoryId)
        {
            while (parentId != null)
            {
                if (parentId == categoryId)
                {
                    return true; // Vòng lặp phát hiện
                }

                var parentCategory = _context.Categories.Find(parentId);
                parentId = parentCategory?.ParentCategoryId;
            }
            return false;
        }
        [HttpPost]
        public IActionResult AddCategory(string categoryName, int? parentCategory)
        {
            if (string.IsNullOrEmpty(categoryName))
            {
                ModelState.AddModelError("CategoryName", "Tên danh mục không được để trống.");
                return RedirectToAction("QuanLyDanhMuc");
            }

            if (categoryName.Length > 255)
            {
                ModelState.AddModelError("CategoryName", "Tên danh mục không được dài quá 255 ký tự.");
                return RedirectToAction("QuanLyDanhMuc");
            }

            // Kiểm tra trùng lặp tên danh mục
            if (_context.Categories.Any(c => c.CategoryName == categoryName))
            {
                ModelState.AddModelError("CategoryName", "Tên danh mục đã tồn tại.");
                return RedirectToAction("QuanLyDanhMuc");
            }

            // Kiểm tra danh mục cha hợp lệ
            if (parentCategory != null && !_context.Categories.Any(c => c.CategoryId == parentCategory))
            {
                ModelState.AddModelError("ParentCategory", "Danh mục cha không hợp lệ.");
                return RedirectToAction("QuanLyDanhMuc");
            }

            // Tạo slug tự động từ categoryName
            string slug = GenerateSlug(categoryName);

            // Kiểm tra trùng lặp Slug
            if (_context.Categories.Any(c => c.Slug == slug))
            {
                ModelState.AddModelError("Slug", "Slug đã tồn tại.");
                return RedirectToAction("QuanLyDanhMuc");
            }

            var newCategory = new Category
            {
                CategoryName = categoryName,
                Slug = slug,
                ParentCategoryId = parentCategory,
                Status = true // Mặc định là hiện
            };

            _context.Categories.Add(newCategory);
            _context.SaveChanges();
            return RedirectToAction("QuanLyDanhMuc");
        }

        // Xóa danh mục
        [HttpPost]
        public IActionResult ConfirmDeleteCategory(int id)
        {
            var category = _context.Categories.Include(c => c.Products).FirstOrDefault(c => c.CategoryId == id);
            if (category == null)
            {
                return Json(new { success = false, message = "Danh mục không tồn tại." });
            }

            var relatedProductsCount = category.Products?.Count ?? 0;

            return Json(new
            {
                success = true,
                message = "Nếu xóa danh mục này sẽ ảnh hưởng đến các sản phẩm liên quan.",
                relatedProductsCount
            });
        }

        [HttpPost]
        public IActionResult DeleteCategoryConfirmed(int id)
        {
            var childCategories = _context.Categories.Where(c => c.ParentCategoryId == id).ToList();

            foreach (var child in childCategories)
            {
                DeleteCategoryConfirmed(child.CategoryId);
            }

            var category = _context.Categories.Find(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                _context.SaveChanges();
            }

            return Json(new { success = true, message = "Danh mục đã được xóa thành công!" });
        }

        // Hiển thị popup sửa danh mục
        public IActionResult EditCategory(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            return PartialView("_EditCategory", category);
        }

        // Cập nhật danh mục
        [HttpPost]
        [Route("/Admin/DanhMuc/UpdateCategory")]
        public IActionResult UpdateCategory(int categoryId, string categoryName, int? parentCategory, bool status)
        {
            var category = _context.Categories.Find(categoryId);
            if (category == null)
            {
                return Json(new { success = false, message = "Không tìm thấy danh mục cần cập nhật." });
            }

            if (string.IsNullOrEmpty(categoryName))
            {
                return Json(new { success = false, message = "Tên danh mục không được để trống." });
            }

            if (categoryName.Length > 255)
            {
                return Json(new { success = false, message = "Tên danh mục không được dài quá 255 ký tự." });
            }

            // Kiểm tra trùng lặp tên danh mục
            if (_context.Categories.Any(c => c.CategoryName == categoryName && c.CategoryId != categoryId))
            {
                return Json(new { success = false, message = "Tên danh mục đã tồn tại." });
            }

            // Kiểm tra danh mục cha hợp lệ
            if (parentCategory != null && !_context.Categories.Any(c => c.CategoryId == parentCategory))
            {
                return Json(new { success = false, message = "Danh mục cha không hợp lệ." });
            }

            // Kiểm tra vòng lặp danh mục cha - con
            if (HasCircularReference(parentCategory, categoryId))
            {
                return Json(new { success = false, message = "Không thể đặt danh mục cha tạo vòng lặp." });
            }

            // Tạo slug và kiểm tra trùng lặp
            string slug = GenerateSlug(categoryName);
            if (_context.Categories.Any(c => c.Slug == slug && c.CategoryId != categoryId))
            {
                return Json(new { success = false, message = "Slug đã tồn tại." });
            }

            try
            {
                category.CategoryName = categoryName;
                category.Slug = slug; // Tạo slug từ categoryName
                category.ParentCategoryId = parentCategory;
                category.Status = status;

                _context.Categories.Update(category);
                _context.SaveChanges();

                return Json(new { success = true, message = "Danh mục đã được cập nhật thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi trong quá trình cập nhật danh mục.", error = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult ToggleCategoryStatus(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }

            // Đảo trạng thái của danh mục
            category.Status = !category.Status;

            _context.Categories.Update(category);
            _context.SaveChanges();

            return Json(new { success = true, message = "Trạng thái danh mục đã được cập nhật!" });
        }

        // Hàm tạo Slug từ CategoryName
        private string GenerateSlug(string categoryName)
        {
            return categoryName.ToLower()
                               .Trim()
                               .Replace(" ", "-")   // Thay khoảng trắng bằng dấu gạch ngang
                               .Replace(",", "")    // Loại bỏ dấu phẩy
                               .Replace(".", "")    // Loại bỏ dấu chấm
                               .Replace("đ", "d")   // Thay chữ 'đ' bằng 'd'
                               .Replace("á", "a")
                               .Replace("à", "a")
                               .Replace("ạ", "a")
                               .Replace("ả", "a")
                               .Replace("ã", "a")
                               .Replace("â", "a")
                               .Replace("ấ", "a")
                               .Replace("ậ", "a")
                               .Replace("ầ", "a")
                               .Replace("ẩ", "a")
                               .Replace("ẫ", "a")
                               .Replace("ă", "a")
                               .Replace("ắ", "a")
                               .Replace("ặ", "a")
                               .Replace("ằ", "a")
                               .Replace("ẳ", "a")
                               .Replace("ẵ", "a")
                               // Thêm các ký tự cần chuyển đổi khác nếu cần
                               .Normalize();        // Chuẩn hóa chuỗi
        }
    }
}
