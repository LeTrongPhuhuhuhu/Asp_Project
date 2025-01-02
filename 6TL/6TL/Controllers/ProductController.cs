using _6TL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
			var product = _context.Products.FirstOrDefault(p => (id.HasValue && p.ProductId == id) || (!string.IsNullOrEmpty(slug) && p.Slug == slug));

			if (product == null)
			{
				return NotFound();
			}
			ViewData["Color"] = product.Color;

			return View(product);
		}
		[HttpPost]
		public JsonResult AddToCart(int productId, string productName, string productImage, decimal productPrice, string productColor, int quantity)
		{
			try
			{
				// Lấy thông tin người dùng (Giả sử bạn có một phương thức để lấy userId)
				var customerId = 1; // Hoặc session, cookie...

				// Kiểm tra giỏ hàng có sản phẩm này chưa
				var cartItem = _context.Carts.FirstOrDefault(c => c.CustomerId == customerId && c.ProductId == productId);

				if (cartItem != null)
				{
					// Nếu sản phẩm đã có trong giỏ hàng, cập nhật số lượng
					cartItem.Quantity += quantity;
					_context.SaveChanges();
				}
				else
				{
					// Nếu chưa có, thêm mới sản phẩm vào giỏ hàng
					var newItem = new Cart
					{
						CustomerId = customerId,
						ProductId = productId,
						ProductName = productName,
						ProductImage = productImage,
						Price = productPrice,
						Color = productColor,
						Quantity = quantity,
						CreatedAt = DateTime.Now
					};
					_context.Carts.Add(newItem);
					_context.SaveChanges();
				}

				return Json(new { success = true });
			}
			catch (Exception ex)
			{
				// Xử lý lỗi
				return Json(new { success = false, message = ex.Message });
			}
		}



	}
}
