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
        public IActionResult SanPham(string? search, List<string>? categories, decimal? minPrice, decimal? maxPrice)
        {
            var products = _context.Products.AsQueryable();

            // Apply category filter
            if (categories != null && categories.Any())
            {
                products = products.Where(p => categories.Contains(p.Category.CategoryName));
            }

            // Apply search filter for product name or description
            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(p => p.ProductName.Contains(search) || p.ProductDescription.Contains(search));
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

            ViewBag.Categories = _context.Categories.ToList();

            return View(products.ToList());
        }

    }
}
