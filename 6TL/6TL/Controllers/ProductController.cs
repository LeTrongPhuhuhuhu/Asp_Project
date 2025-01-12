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

    // Lấy danh sách màu sắc và số lượng liên quan đến sản phẩm từ bảng Products (hoặc một bảng khác nếu cần)
    var colorsWithQuantity = _context.Products
        .Where(p => p.ProductId == product.ProductId)
        .Select(p => new
        {
            p.Color,   // Giả sử màu sắc đã được lưu trữ trong cột ColorCode trong bảng Products
            p.Quantity     // Giả sử số lượng tồn kho đã được lưu trữ trong cột Quantity trong bảng Products
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

				// Sử dụng customerId = 1 tạm thời
				int customerId = 1;

				// Kiểm tra sản phẩm đã có trong giỏ hàng của customerId = 1 hay chưa
				var existingCart = _context.Carts
					.FirstOrDefault(c => c.ProductId == productId && c.Color == productColor && c.CustomerId == customerId);

				if (existingCart != null)
				{
					// Cập nhật số lượng và tổng giá trị của sản phẩm
					existingCart.Quantity += quantity;
					existingCart.TotalPrice = existingCart.Price * existingCart.Quantity;
					existingCart.UpdatedAt = DateTime.Now;
				}
				else
				{
					// Thêm sản phẩm mới vào giỏ hàng
					var newCart = new Cart
					{
						ProductId = productId,
						ProductName = productName,
						ProductImage = productImage,
						Price = price,
						Quantity = quantity,
						Color = productColor,
						TotalPrice = price * quantity,
						CustomerId = customerId, // customerId = 1
						CreatedAt = DateTime.Now
					};

					_context.Carts.Add(newCart);
				}

				// Lưu các thay đổi vào cơ sở dữ liệu
				_context.SaveChanges();

				// Thông báo thành công đơn giản
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
