using _6TL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace _6TL.Areas.Admin.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Đảm bảo đường dẫn chính xác
    public class CategoryApiController : ControllerBase
    {
        private readonly Db6TLContext _context;

        public CategoryApiController(Db6TLContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory([FromBody] Category category)
        {
            if (category == null || string.IsNullOrWhiteSpace(category.CategoryName))
            {
                return BadRequest(new { success = false, message = "Tên danh mục không được để trống." });
            }

            try
            {
                // Tạo slug tự động từ CategoryName
                category.Slug = GenerateSlug(category.CategoryName);

                // Thêm danh mục vào cơ sở dữ liệu
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Danh mục đã được thêm thành công." });
            }
            catch (Exception ex)
            {
                // Thêm thông báo lỗi chi tiết để dễ dàng kiểm tra
                return StatusCode(500, new { success = false, message = "Có lỗi xảy ra, vui lòng thử lại.", error = ex.Message });
            }
        }

        // Hàm tạo slug từ tên danh mục
        private string GenerateSlug(string name)
        {
            // Chuyển tên thành chữ thường và thay thế dấu cách bằng dấu gạch ngang
            var slug = name.ToLower();
            slug = Regex.Replace(slug, @"\s+", "-"); // Thay khoảng trắng bằng dấu gạch ngang
            slug = Regex.Replace(slug, @"[^a-z0-9-]", ""); // Loại bỏ ký tự đặc biệt

            return slug;
        }
    }
}
