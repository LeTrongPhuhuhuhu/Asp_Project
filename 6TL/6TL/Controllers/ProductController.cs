using _6TL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace _6TL.Controllers
{
	public class ProductController : Controller
	{
		private readonly Db6tlContext _context;

		public ProductController(Db6tlContext context)
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

			var colors = _context.ProductColors
				.Where(pc => pc.ProductId == product.ProductId)
				.Join(_context.Colors, pc => pc.ColorId, c => c.ColorId, (pc, c) => c)
				.ToList();

			ViewData["Colors"] = colors;

			return View(product);
		}

		[HttpPost]
		public IActionResult AddToCart(int customerId, int productId, string productName, string? productImage, decimal price, string color, int quantity)
		{
			try
			{
				// Đặt giá trị mặc định cho 'color' nếu nó là null
				color = string.IsNullOrEmpty(color) ? "Màu mặc định" : color;

				// Kiểm tra xem productName có giá trị hay không
				if (string.IsNullOrEmpty(productName))
				{
					return Json(new { success = false, message = "Tên sản phẩm không được để trống." });
				}

				Console.WriteLine("customerId: " + customerId);
				Console.WriteLine("productId: " + productId);
				Console.WriteLine("productName: " + productName); // Log productName để kiểm tra
				Console.WriteLine("productImage: " + productImage);
				Console.WriteLine("price: " + price);
				Console.WriteLine("color: " + color);
				Console.WriteLine("quantity: " + quantity);

				// Kiểm tra xem sản phẩm đã có trong giỏ hàng của khách hàng chưa
				var existingCartItem = _context.Carts
					.FirstOrDefault(c => c.ProductId == productId && c.Color == color);

				if (existingCartItem != null)
				{
					// Nếu có, chỉ cần cập nhật số lượng và tính lại tổng giá
					existingCartItem.Quantity += quantity; // Tăng số lượng sản phẩm
					existingCartItem.TotalPrice = existingCartItem.Price * existingCartItem.Quantity; // Tính lại tổng giá
					existingCartItem.UpdatedAt = DateTime.Now; // Cập nhật thời gian sửa đổi
				}
				else
				{
					// Nếu chưa có, tạo mới sản phẩm trong giỏ hàng
					var cartItem = new Cart
					{
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
				_context.SaveChanges();

				// Trả về kết quả JSON thành công
				return Json(new { success = true, message = "Sản phẩm đã được thêm vào giỏ hàng!" });
			}
			catch (Exception ex)
			{
				// Log chi tiết lỗi để kiểm tra
				var innerException = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
				return Json(new { success = false, message = "Lỗi roi: " + innerException });
			}
		}




	}
}
