using _6TL.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace _6TL.Controllers
{
	public class ProductController : Controller
	{
		private readonly Db6TLContext _context;

		public ProductController(Db6TLContext context)
		{
			_context = context;
		}

		public IActionResult ChiTietSanPham(int id)
		{
			var product = _context.Products.FirstOrDefault(p => p.ProductId == id);
			if (product == null)
			{
				return NotFound(); // Trả về 404 nếu không tìm thấy sản phẩm
			}

			// Truyền mã màu vào View (nếu có)
			ViewData["Color"] = product.Color;

			return View(product);
		}
	}
}
