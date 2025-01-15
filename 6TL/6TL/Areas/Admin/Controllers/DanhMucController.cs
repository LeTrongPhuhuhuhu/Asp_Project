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
        //Thêm danh mục mới
        [HttpPost]
        public IActionResult AddCategory(string categoryName, int? parentCategory)
        {
            if (string.IsNullOrEmpty(categoryName))
            {
                ModelState.AddModelError("CategoryName", "Tên danh mục không được để trống.");
                return RedirectToAction("QuanLyDanhMuc");
            }

            // Tạo slug tự động từ categoryName
            string slug = GenerateSlug(categoryName);

            var newCategory = new Category
            {
                CategoryName = categoryName,
                Slug = slug,
                ParentCategoryId = parentCategory
            };

            _context.Categories.Add(newCategory);
            _context.SaveChanges();
            return RedirectToAction("QuanLyDanhMuc");
        }


        // Xóa danh mục
        public IActionResult DeleteCategory(int id)
        {
            var childCategories = _context.Categories.Where(c => c.ParentCategoryId == id).ToList();

            foreach (var child in childCategories)
            {
                DeleteCategory(child.CategoryId);
            }

            var category = _context.Categories.Find(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                _context.SaveChanges();
            }

            return RedirectToAction("QuanLyDanhMuc");
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
        public IActionResult UpdateCategory(int id, string categoryName, int? parentCategory, bool status)
        {
            var category = _context.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(categoryName))
            {
                return Json(new { success = false, message = "Tên danh mục không được để trống." });
            }

            category.CategoryName = categoryName;
            category.Slug = GenerateSlug(categoryName); // Tạo lại slug tự động
            category.ParentCategoryId = parentCategory;
            category.Status = status; // Update status

            _context.Categories.Update(category);
            _context.SaveChanges();

            return Json(new { success = true, message = "Danh mục đã được cập nhật thành công!" });
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
