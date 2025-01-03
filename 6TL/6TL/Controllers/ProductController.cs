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
		public IActionResult AddToCart(int customerId, int productId, string productName, string? productImage, decimal price, string color, int quantity)
		{
			try
			{
				// Kiểm tra xem sản phẩm đã có trong giỏ hàng của khách hàng chưa
				var existingCartItem = _context.Carts
					.FirstOrDefault(c => c.CustomerId == customerId && c.ProductId == productId);

				if (existingCartItem != null)
				{
					// Nếu có, chỉ cần cập nhật số lượng và tính lại tổng giá
					Console.WriteLine("Sản phẩm đã tồn tại trong giỏ hàng, cập nhật số lượng.");
					existingCartItem.Quantity += quantity; // Tăng số lượng sản phẩm
					existingCartItem.TotalPrice = existingCartItem.Price * existingCartItem.Quantity; // Tính lại tổng giá
					existingCartItem.UpdatedAt = DateTime.Now; // Cập nhật thời gian sửa đổi
				}
				else
				{
					// Nếu chưa có, tạo mới sản phẩm trong giỏ hàng
					Console.WriteLine("Sản phẩm chưa tồn tại trong giỏ hàng, thêm sản phẩm mới.");
					var cartItem = new Cart
					{
						CustomerId = 1,
						ProductId = productId,
						ProductName = productName,
						ProductImage = productImage,
						Price = price,
						Quantity = quantity,
						TotalPrice = price * quantity, // Tính tổng giá ngay khi thêm mới
						Color = color,
						CreatedAt = DateTime.Now,
						UpdatedAt = DateTime.Now
					};
					_context.Carts.Add(cartItem); // Thêm vào giỏ hàng
				}

				// Lưu lại thay đổi trong cơ sở dữ liệu
				Console.WriteLine("Lưu lại thay đổi trong cơ sở dữ liệu.");
				_context.SaveChanges();

				// Trả về kết quả JSON thành công
				Console.WriteLine("Sản phẩm đã được thêm vào giỏ hàng!");
				return Json(new { success = true, message = "Sản phẩm đã được thêm vào giỏ hàng!" });
			}
			catch (Exception ex)
			{
				// Nếu có lỗi, trả về thông báo lỗi
				Console.WriteLine("Có lỗi xảy ra: " + ex.Message);
				return Json(new { success = false, message = "Lỗi: " + ex.Message });
			}
		}






	}
}
