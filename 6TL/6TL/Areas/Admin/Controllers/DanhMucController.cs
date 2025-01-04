using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _6TL.Models; // Đảm bảo thêm namespace của model
using System.Linq;

namespace _6TL.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DanhMucController : Controller
    {
        private readonly Db6TLContext _context;

        public DanhMucController(Db6TLContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách danh mục
        public IActionResult QuanLyDanhMuc(string searchQuery)
        {
            var categories = _context.Categories
                                     .Where(c => string.IsNullOrEmpty(searchQuery) || c.CategoryName.Contains(searchQuery))
                                     .ToList();
            return View(categories);
        }

        // Thêm danh mục mới
        [HttpPost]
        public IActionResult AddCategory(string categoryName, int? parentCategory)
        {
            if (string.IsNullOrEmpty(categoryName))
            {
                ModelState.AddModelError("CategoryName", "Tên danh mục không được để trống.");
                return View();
            }

            var newCategory = new Category
            {
                CategoryName = categoryName,
                Slug = GenerateSlug(categoryName), // Tạo slug từ categoryName
                ParentCategoryId = parentCategory
            };

            _context.Categories.Add(newCategory);
            _context.SaveChanges();
            return RedirectToAction("QuanLyDanhMuc");
        }

        // Xóa danh mục
        public IActionResult DeleteCategory(int id)
        {
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
        public IActionResult UpdateCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Update(category);
                _context.SaveChanges();
                return RedirectToAction("QuanLyDanhMuc");
            }
            return View(category);
        }

        // Hàm tạo Slug từ CategoryName
        private string GenerateSlug(string categoryName)
        {
            return categoryName.ToLower().Replace(" ", "-").Replace(",", "").Replace(".", "");
        }
    }
}
