using _6TL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _6TL.Areas.Admin.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SupplierApiController : ControllerBase
    {
        private readonly Db6TLContext _context;

        public SupplierApiController(Db6TLContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddSupplier([FromBody] Supplier supplier)
        {
            if (supplier == null || string.IsNullOrWhiteSpace(supplier.SupplierName))
            {
                return BadRequest(new { success = false, message = "Tên nhà cung cấp không được để trống." });
            }

            try
            {
                // Thêm nhà cung cấp mới vào cơ sở dữ liệu
                _context.Suppliers.Add(supplier);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Nhà cung cấp đã được thêm thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Có lỗi xảy ra, vui lòng thử lại." });
            }
        }
      
    }

}
