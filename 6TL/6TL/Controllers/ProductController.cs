using _6TL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
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

		public IActionResult ChiTietSanPham(int? id, string slug)
		{
			var product = _context.Products
				.FirstOrDefault(p => (id.HasValue && p.ProductId == id) || (!string.IsNullOrEmpty(slug) && p.Slug == slug));

			if (product == null)
			{
				return NotFound();
			}

			// Lấy danh sách màu sắc liên quan đến sản phẩm
			var colors = _context.ProductColors
				.Where(pc => pc.ProductId == product.ProductId)
				.Join(_context.Colors, pc => pc.ColorId, c => c.ColorId, (pc, c) => c)
				.ToList();

			ViewData["Colors"] = colors;

			// Lấy sản phẩm liên quan theo cùng loại (category)
			var relatedProducts = _context.Products
				.Where(p => p.CategoryId == product.CategoryId && p.ProductId != product.ProductId)
				.Take(8) // Lấy tối đa 4 sản phẩm liên quan
				.ToList();

			ViewData["RelatedProducts"] = relatedProducts;

			return View(product);
		}

		[HttpPost]
		public IActionResult AddToCart(int productId, string productName, string productImage, decimal price, int quantity, string productColor)
		{
			try
			{
				if (string.IsNullOrEmpty(productColor))
				{
					return Json(new { success = false, message = "Vui lòng chọn màu sắc!" });
				}

				var existingCart = _context.Carts
					.FirstOrDefault(c => c.ProductId == productId && c.Color == productColor);

				if (existingCart != null)
				{
					existingCart.Quantity += quantity;
					existingCart.TotalPrice = existingCart.Price * existingCart.Quantity;
					existingCart.UpdatedAt = DateTime.Now;
				}
				else
				{
					var newCart = new Cart
					{
						ProductId = productId,
						ProductName = productName,
						ProductImage = productImage,
						Price = price,
						Quantity = quantity,
						Color = productColor,
						TotalPrice = price * quantity,
						CreatedAt = DateTime.Now
					};

					_context.Carts.Add(newCart);
				}

				_context.SaveChanges();

				return Json(new { success = true, message = "Sản phẩm đã được thêm vào giỏ hàng!" });
			}
			catch (Exception ex)
			{
				// Log lỗi nếu cần
				return Json(new { success = false, message = "Đã có lỗi xảy ra khi thêm sản phẩm vào giỏ hàng." });
			}
		}



	}
}
