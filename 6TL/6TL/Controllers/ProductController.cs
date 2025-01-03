using _6TL.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace _6TL.Controllers
{
	public class ProductController : Controller
	{
		private readonly Db6TlContext _context;

		public ProductController(Db6TlContext context)
		{
			_context = context;
		}

		public IActionResult ChiTietSanPham(int? id, string slug)
		{
			var product = _context.Products.FirstOrDefault(p => (id.HasValue && p.ProductId == id) || (!string.IsNullOrEmpty(slug) && p.Slug == slug));

			if (product == null)
			{
				return NotFound();
			}
			ViewData["Color"] = product.Color;

			return View(product);
		}

		[HttpPost]
		public IActionResult AddToCart(int productId, string productName, string productImage, decimal productPrice, string productColor, int quantity)
		{
			var existingCartItem = _context.Carts.FirstOrDefault(c => c.ProductId == productId);

			if (existingCartItem != null)
			{
				// Nếu sản phẩm đã có trong giỏ hàng, tăng số lượng sản phẩm
				existingCartItem.Quantity += quantity;
				existingCartItem.TotalPrice = existingCartItem.Quantity * existingCartItem.Price;
			}
			else
			{
				// Nếu chưa có, thêm sản phẩm mới vào giỏ hàng
				var cartItem = new Cart
				{
					ProductId = productId,
					ProductName = productName,
					ProductImage = productImage,
					Price = productPrice,
					Quantity = quantity,
					TotalPrice = productPrice * quantity,
					CreatedAt = DateTime.Now,
					UpdatedAt = DateTime.Now
				};

				_context.Carts.Add(cartItem);
			}

			_context.SaveChanges();

			return Json(new { success = true, message = "Product added to cart" });
		}



	}
}
