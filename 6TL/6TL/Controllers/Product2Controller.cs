using _6TL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace _6TL.Controllers
{
    public class Product2Controller : Controller
    {
        private readonly Db6TLContext _context;

        public Product2Controller(Db6TLContext context)
        {
            _context = context;
        }

        public ActionResult SanPhamYeuThich()
        {
            return View();
        }
        [HttpGet]
        public ActionResult LaySanPhamYeuThich(int customerId)
        {
            var wishlist = _context.Wishlists
                .Where(w => w.CustomerId == customerId)
                .Select(w => new Wishlist
                {
                    ProductId = w.ProductId ?? 0,
                    ProductName = w.ProductName,
                    Price = w.Price ?? 0,
                    ProductImage = w.ProductImage,
                    Rating = w.Rating
                }).ToList();

            return Json(wishlist);
        }
        [HttpGet]
        public IActionResult SanPham(string? search, List<string>? categories, decimal? minPrice, decimal? maxPrice, int page = 1, int pageSize = 9)
        {
            var products = _context.Products.AsQueryable();

            // Kiểm tra từ khóa tìm kiếm có hợp lệ không
            if (!string.IsNullOrEmpty(search))
            {
                // Kiểm tra nếu từ khóa chứa ký tự không hợp lệ (ký tự đặc biệt, dấu cách, hoặc rỗng)
                if (string.IsNullOrWhiteSpace(search) || !search.All(c => char.IsLetterOrDigit(c) || char.IsLetter(c) || char.IsWhiteSpace(c)))
                {
                    TempData["Message"] = "Vui lòng nhập từ khóa hợp lệ để tìm kiếm."; // Thông báo nếu từ khóa không hợp lệ
                    ViewBag.Categories = _context.Categories.ToList(); // Lấy danh sách danh mục
                    return View("SanPham", new List<Product>()); // Trả về view với danh sách sản phẩm rỗng
                }

                // Apply search filter for product name or description
                products = products.Where(p => p.ProductName.Contains(search) || p.ProductDescription.Contains(search));
            }

            // Apply category filter
            if (categories != null && categories.Any())
            {
                products = products.Where(p => categories.Contains(p.Category.CategoryName));
            }

            // Apply price filter
            if (minPrice.HasValue)
            {
                products = products.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                products = products.Where(p => p.Price <= maxPrice.Value);
            }

            // Total products after filtering
            var totalProducts = products.Count();
            var totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);

            // Apply pagination
            var productsPaged = products.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            // Set up pagination information
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.Search = search;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.BaseUrl = Url.Action("SanPham", "Product2");
            return View(productsPaged);
        }




    }
}
