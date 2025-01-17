using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _6TL.Models;
using _6TL.ViewModels;

namespace _6TL.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class ReviewController : Controller
	{

		private readonly Db6TLContext _context;

		public ReviewController(Db6TLContext context)
		{
			_context = context;
		}

		// Hiển thị danh sách sản phẩm và đánh giá
		public async Task<IActionResult> QuanLyDanhGia()
		{
			var products = await _context.Products
				.Include(p => p.Reviews)
				.Select(p => new Product
				{
					ProductId = p.ProductId,
					ProductName = p.ProductName,
					Image = p.Image,
					Price = p.Price,
					Rating = p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0,
					Quantity = p.Quantity,
					Reviews = p.Reviews
				}).ToListAsync();

			var reviews = await _context.Reviews.ToListAsync();

			var viewModel = new ReviewManagementViewModel
			{
				Products = products,
				Reviews = reviews
			};

			return View(viewModel);
		}

		// Lọc đánh giá theo số sao
		[HttpGet]
		public async Task<IActionResult> SortProductsByRating(string order, int page = 1)
		{
			// Define page size
			int pageSize = 5;

			var productsQuery = _context.Products.AsQueryable();

			// Apply sorting based on the order parameter
			if (order == "asc")
			{
				productsQuery = productsQuery.OrderBy(p => p.Rating);  // Sort in ascending order
			}
			else
			{
				productsQuery = productsQuery.OrderByDescending(p => p.Rating);  // Sort in descending order
			}

			// Pagination
			var totalProducts = await productsQuery.CountAsync();
			var pagedProducts = await productsQuery
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			var totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);

			var response = new
			{
				products = pagedProducts,
				totalPages = totalPages
			};

			return Json(response);
		}

		[HttpGet]
		public async Task<IActionResult> GetPagedProducts(string order = "asc", int page = 1, int pageSize = 5)
		{
			var productsQuery = _context.Products.AsQueryable();

			// Sort by average rating
			productsQuery = order.ToLower() == "asc"
				? productsQuery.OrderBy(p => p.Reviews.Average(r => r.Rating))
				: productsQuery.OrderByDescending(p => p.Reviews.Average(r => r.Rating));

			// Pagination
			int totalProducts = await productsQuery.CountAsync();
			var pagedProducts = await productsQuery
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			// Return data as JSON
			var totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);
			var response = new
			{
				products = pagedProducts,
				totalPages = totalPages
			};
			return Json(response);
		}

		[HttpGet]
		public IActionResult GetChartData()
		{
			var data = _context.Reviews
				.GroupBy(r => (int)Math.Round(r.Rating)) // Làm tròn đến số nguyên
				.OrderByDescending(g => g.Count())       // Sắp xếp theo số lượng giảm dần
				.Select(g => new
				{
					Rating = g.Key,
					Count = g.Count()
				})
				.ToList();

			// Chuẩn bị dữ liệu biểu đồ
			var chartData = data.Select(d => d.Count).ToList();
			return Json(chartData);
		}
		[HttpPost]
		public async Task<IActionResult> DeleteReview([FromBody] int reviewId)
		{
			var review = await _context.Reviews
				.Where(r => r.ReviewId == reviewId)
				.FirstOrDefaultAsync();

			if (review == null)
			{
				Console.WriteLine("Review not found with ID: " + reviewId);
				return Json(new { success = false, message = "Bình luận không tồn tại." });
			}

			try
			{
				// Xóa bình luận
				_context.Reviews.Remove(review);
				await _context.SaveChangesAsync();

				// Trả về phản hồi thành công
				return Json(new { success = true, message = "Bình luận đã được xóa thành công." });
			}
			catch (Exception ex)
			{
				// Nếu có lỗi xảy ra
				return Json(new { success = false, message = "Có lỗi xảy ra khi xóa bình luận: " + ex.Message });
			}
		}
	}
}
