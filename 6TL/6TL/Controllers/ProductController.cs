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

			// Lấy danh sách màu sắc và số lượng liên quan đến sản phẩm
			var colorsWithQuantity = _context.ProductColors
				.Where(pc => pc.ProductId == product.ProductId)
				.Join(_context.Colors,
					  pc => pc.ColorId,
					  c => c.ColorId,
					  (pc, c) => new
					  {
						  c.ColorId,
						  c.ColorCode,
						  pc.Quantity
					  })
				.ToList();
			ViewBag.ProductRating = product.Rating; // Truyền rating từ bảng Product

			ViewBag.ColorsWithQuantity = colorsWithQuantity;

			var relatedProducts = _context.Products
				.Where(p => p.CategoryId == product.CategoryId && p.ProductId != product.ProductId)
				.Take(8)
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

				// Tính tổng số lượng sản phẩm trong giỏ hàng
				var totalQuantity = _context.Carts.Sum(c => c.Quantity);

				return Json(new { success = true, message = "Sản phẩm đã được thêm vào giỏ hàng!", totalQuantity });
			}
			catch (Exception ex)
			{
				// Log lỗi nếu cần
				return Json(new { success = false, message = "Đã có lỗi xảy ra khi thêm sản phẩm vào giỏ hàng." });
			}
		}




	}
}
