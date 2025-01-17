using Microsoft.AspNetCore.Mvc;
using _6TL.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace _6TL.Controllers
{
	[Area("Admin")]
	public class OrdersController : Controller
	{
		private readonly Db6TLContext _context;

		public OrdersController(Db6TLContext context)
		{
			_context = context;
		}

		public IActionResult QuanLyDonHang()
		{
			var orders = _context.Orders.Select(o => new
			{
				o.OrderId,
				o.CustomerName,
				o.PhoneNumber,
				OrderDate = o.OrderDate.HasValue ? o.OrderDate.Value.ToString("dd/MM/yyyy") : "",
				o.OrderStatus,
				o.PaymentMethod,
				TotalAmount = o.TotalAmount.HasValue ? o.TotalAmount.Value.ToString("C") : "0"
			}).ToList();

			return View(orders);
		}

		[HttpGet]
		public async Task<IActionResult> QuanLyChiTietDonHang(int orderId)
		{
			var order = await _context.Orders
				.Include(o => o.OrderDetails)
				.ThenInclude(od => od.Product)
				.FirstOrDefaultAsync(o => o.OrderId == orderId);

			if (order == null)
			{
				return NotFound();
			}

			return View(order);
		}
		[HttpPost]
		public async Task<IActionResult> UpdateOrderStatus(int orderId, string newStatus)
		{
			try
			{
				var order = await _context.Orders.FindAsync(orderId);
				if (order == null)
				{
					return Json(new { success = false, message = "Không tìm thấy đơn hàng" });
				}

				// Kiểm tra tính hợp lệ của trạng thái mới
				if (!IsValidStatusTransition(order.OrderStatus, newStatus))
				{
					return Json(new { success = false, message = "Trạng thái không hợp lệ" });
				}

				order.OrderStatus = newStatus;
				await _context.SaveChangesAsync();

				return Json(new { success = true });
			}
			catch (Exception ex)
			{
				return Json(new { success = false, message = ex.Message });
			}
		}

		private bool IsValidStatusTransition(string currentStatus, string newStatus)
		{
			// Kiểm tra các chuyển đổi trạng thái hợp lệ
			switch (currentStatus)
			{
				case "Chờ xử lý":
					return newStatus == "Đang xử lý" || newStatus == "Đã hủy";
				case "Đang xử lý":
					return newStatus == "Hoàn thành" || newStatus == "Đã hủy";
				default:
					return false;
			}
		}
	}
}