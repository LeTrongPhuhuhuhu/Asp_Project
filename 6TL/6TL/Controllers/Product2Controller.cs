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
        public IActionResult SanPham(List<string> categories, List<string> materials, decimal? minPrice, decimal? maxPrice)
        {
            var products = _context.Products.AsQueryable();

            // Apply category filter
            if (categories != null && categories.Any())
            {
                products = products.Where(p => categories.Contains(p.Category.CategoryName));
            }

            // Apply material filter
            if (materials != null && materials.Any())
            {
                products = products.Where(p => materials.Contains(p.Material));
            }

            // Apply price range filter
            if (minPrice.HasValue)
            {
                products = products.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                products = products.Where(p => p.Price <= maxPrice.Value);
            }

            // Passing categories and materials to the view to display the checkboxes
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Materials = _context.Products
                .Select(p => p.Material)
                .Distinct()
                .ToList();

            return View(products.ToList());
        }
    }
}
