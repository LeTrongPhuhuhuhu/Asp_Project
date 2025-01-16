using Microsoft.AspNetCore.Mvc;
using _6TL.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace _6TL.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderManagementController : Controller
    {
        private readonly Db6tlContext _context;

        public OrderManagementController(Db6tlContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách đơn hàng
        public IActionResult QuanLyDonHang(string status, string date, string orderId)
        {
            var orders = _context.Orders.AsQueryable();

            // Lọc theo trạng thái
            if (!string.IsNullOrEmpty(status))
            {
                orders = orders.Where(o => o.OrderStatus == status);
            }

            // Lọc theo ngày tạo
            if (!string.IsNullOrEmpty(date) && DateTime.TryParse(date, out DateTime filterDate))
            {
                orders = orders.Where(o => o.OrderDate.Value.Date == filterDate.Date);
            }

            // Lọc theo mã đơn hàng
            if (!string.IsNullOrEmpty(orderId))
            {
                orders = orders.Where(o => o.OrderId.ToString().Contains(orderId));
            }

            return View(orders.ToList()); // Trả về danh sách đơn hàng đã lọc
        }


        // Xem chi tiết đơn hàng
        public IActionResult Details(int orderId)
        {
            var order = _context.Orders
                .Where(o => o.OrderId == orderId)
                .Select(o => new
                {
                    o.OrderId,
                    o.CustomerName,
                    o.OrderDate,
                    o.TotalAmount,
                    o.OrderStatus,
                    OrderDetails = o.OrderDetails.Select(od => new
                    {
                        od.ProductName,
                        od.UnitPrice,
                        od.Quantity
                    })
                })
                .FirstOrDefault();

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpPost]
        public IActionResult UpdateOrderStatus([FromBody] OrderUpdateDto updateData)
        {
            var order = _context.Orders.FirstOrDefault(o => o.OrderId == updateData.OrderId);
            if (order == null)
            {
                return Json(new { success = false, message = "Không tìm thấy đơn hàng." });
            }

            order.OrderStatus = updateData.Status;
            _context.SaveChanges();

            return Json(new { success = true, message = "Trạng thái đơn hàng đã được cập nhật." });
        }

        public class OrderUpdateDto
        {
            public int OrderId { get; set; }
            public string Status { get; set; }
        }



        [HttpPost]
        public IActionResult DeleteOrder(int orderId)
        {
            if (orderId == 0)
            {
                return Json(new { success = false, message = "Invalid OrderId" });
            }

            var order = _context.Orders.Include(o => o.OrderDetails).FirstOrDefault(o => o.OrderId == orderId);
            if (order == null)
            {
                return Json(new { success = false, message = "Không tìm thấy đơn hàng." });
            }

            // Xóa chi tiết đơn hàng
            _context.OrderDetails.RemoveRange(order.OrderDetails);

            // Xóa đơn hàng
            _context.Orders.Remove(order);

            _context.SaveChanges();

            return Json(new { success = true, message = "Đơn hàng và chi tiết đã được xóa thành công." });
        }




    }
}
