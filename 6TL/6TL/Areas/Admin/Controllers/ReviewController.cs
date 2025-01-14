using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _6TL.Models;

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
			var products = _context.Products
			.Select(p => new Product
			{
				ProductId = p.ProductId,
				ProductName = p.ProductName,
				Image = p.Image,
				Price = p.Price,
				Rating = p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0,
				Quantity = p.Quantity,
				Reviews = p.Reviews
			}).ToList();

			var reviews = _context.Reviews.ToList();

			return View(new Tuple<IEnumerable<Product>, IEnumerable<Review>>(products, reviews));
		}

		// Lọc sản phẩm theo giá (Cao nhất/Thấp nhất)
		[HttpGet]
		public IActionResult FilterReviews(int rating)
		{
			var reviews = _context.Reviews
				.Where(r => r.Rating == rating)
				.ToList();

			return PartialView("_ReviewList", reviews);
		}

		// Lọc đánh giá theo số sao
		[HttpPost]
		[HttpGet]
		public IActionResult SortProducts(string order)
		{
			var products = _context.Products
				.Select(p => new Product
				{
					ProductId = p.ProductId,
					ProductName = p.ProductName,
					Image = p.Image,
					Price = p.Price,
					Rating = p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0,
					Quantity = p.Quantity
				})
				.OrderByDescending(p => p.Rating)  // Sort by rating, descending by default
				.ToList();

			if (order == "asc")
			{
				products = products.OrderBy(p => p.Rating).ToList();  // Ascending order for rating
			}

			return PartialView("_ProductList", products);
		}

		// Thêm phản hồi cho một đánh giá
		[HttpPost]
		public async Task<IActionResult> ReplyToReview(int reviewId, string replyText)
		{
			var review = await _context.Reviews.FindAsync(reviewId);

			if (review != null)
			{
				// Thêm logic lưu phản hồi vào cơ sở dữ liệu (nếu cần có bảng phản hồi riêng)
				// Ví dụ: review.Replies.Add(new Reply { Text = replyText });

				await _context.SaveChangesAsync();
				return Json(new { success = true, message = "Phản hồi thành công!" });
			}

			return Json(new { success = false, message = "Không tìm thấy đánh giá!" });
		}
		[HttpGet]
		public IActionResult GetChartData()
		{
			var data = _context.Reviews
				.GroupBy(r => (int)Math.Round(r.Rating))  // Round to nearest integer
				.OrderByDescending(g => g.Key)
				.Select(g => new
				{
					Rating = g.Key,
					Count = g.Count()
				})
				.ToList();

			// Prepare chart data
			var chartData = new List<int> { 0, 0, 0, 0, 0 }; // To hold the counts for 1 to 5 stars

			foreach (var item in data)
			{
				// Ensure only ratings from 1 to 5 are handled
				if (item.Rating >= 1 && item.Rating <= 5)
				{
					chartData[item.Rating - 1] = item.Count;  // Match rating to array index (0 for 1 star, etc.)
				}
			}

			return Json(chartData);
		}
	}
}
