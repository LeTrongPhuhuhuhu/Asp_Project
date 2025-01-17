using _6TL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _6TL.Areas.Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierApiController : ControllerBase
    {
        private readonly Db6TLContext _context;
        public SupplierApiController(Db6TLContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult ThemNhaCC([FromBody] Supplier supplier)
        {
            if (supplier == null)
            {
                return new JsonResult(new { success = false, message = "Dữ liệu không hợp lệ!" });
            }

            try
            {
                // Kiểm tra nếu nhà cung cấp đã tồn tại
                var existingSupplier = _context.Suppliers
                    .FirstOrDefault(s => s.SupplierName.Equals(supplier.SupplierName, StringComparison.OrdinalIgnoreCase));

                if (existingSupplier != null)
                {
                    return new JsonResult(new { success = false, message = "Nhà cung cấp đã tồn tại!" });
                }

                // Thêm mới nhà cung cấp
                supplier.CreatedAt = DateTime.UtcNow;
                _context.Suppliers.Add(supplier);
                _context.SaveChanges();

                return new JsonResult(new
                {
                    success = true,
                    message = "Thêm nhà cung cấp thành công!",
                    data = supplier
                });
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần
                // _logger.LogError(ex, "Lỗi khi thêm nhà cung cấp");
                return new JsonResult(new { success = false, message = "Có lỗi xảy ra, vui lòng thử lại sau!" });
            }
        }
    }
}
