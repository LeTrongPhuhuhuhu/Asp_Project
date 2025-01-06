using _6TL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace _6TL.Controllers
{
    public class Product2Controller : Controller
    {
        private readonly Db6TLContext _context;

        public Product2Controller(Db6TLContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SanPham()
        {
            var category = _context.Categories.ToList();
           
            var products = _context.Products.ToList(); // Lấy tất cả sản phẩm từ database
            var materials = _context.Products
       .Where(p => !string.IsNullOrEmpty(p.Material)) // Bỏ giá trị null hoặc rỗng
       .Select(p => p.Material)
       .Distinct()
       .ToList();

            ViewBag.Categories = category;
            ViewBag.Materials = materials;
            return View(products);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ToggleFavorite(int productId, bool isFavorite)
        {
            try
            {
                // Lấy thông tin người dùng từ Session
                var customerId = HttpContext.Session.GetInt32("CustomerId");
                if (customerId == null)
                {
                    return Json(new { success = false, message = "Người dùng chưa đăng nhập." });
                }

                // Lấy danh sách sản phẩm yêu thích từ session (nếu có)
                var favorites = HttpContext.Session.GetObjectFromJson<List<int>>("Favorites") ?? new List<int>();

                if (isFavorite)
                {
                    // Thêm sản phẩm vào danh sách yêu thích
                    if (!favorites.Contains(productId))
                    {
                        favorites.Add(productId);
                    }
                }
                else
                {
                    // Xóa sản phẩm khỏi danh sách yêu thích
                    favorites.Remove(productId);
                }

                // Lưu lại danh sách yêu thích vào session
                HttpContext.Session.SetObjectAsJson("Favorites", favorites);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public IActionResult SanPhamYeuThich()
        {
            return View(); 
        }
    }

    // Extension methods để lưu trữ đối tượng vào session dưới dạng JSON
    public static class SessionExtensions
    {
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }
    }
}
