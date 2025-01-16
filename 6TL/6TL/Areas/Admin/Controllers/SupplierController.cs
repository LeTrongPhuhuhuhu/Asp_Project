using _6TL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _6TL.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SupplierController : Controller
    {
        private readonly Db6TLContext _context;
        public SupplierController(Db6TLContext context)
        {
            _context = context;
        }
        public IActionResult QuanLyNhaCC()
        {
            var suppliers = _context.Suppliers.ToList(); // Lấy danh sách NCC từ DB
            return View(suppliers);
        }

        [HttpGet]
        public IActionResult ThemNhaCC()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Supplier/ThemNhaCC")]
        public IActionResult ThemNhaCC(Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                supplier.CreatedAt = DateTime.Now; // Gán ngày tạo
                _context.Suppliers.Add(supplier); // Thêm vào DbContext
                _context.SaveChanges(); // Lưu thay đổi vào DB
                return View(); // Chuyển hướng về danh sách (hoặc trang khác)
            }
            return View(); 
        }
        [HttpGet]
        public IActionResult SuaNhaCC(int id)
        {
            var supplier = _context.Suppliers.Find(id);
            if (supplier == null)
            {
                return NotFound();
            }
            return View(supplier);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SuaNhaCC(int id, Supplier supplier)
        {
            if (id != supplier.SupplierId)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    supplier.CreatedAt = DateTime.Now; // Gán ngày sua
                    _context.Update(supplier); // Cập nhật nhà cung cấp
                    _context.SaveChanges();    // Lưu thay đổi vào DB
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Suppliers.Any(e => e.SupplierId == supplier.SupplierId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("QuanLyNhaCC"); // Quay lại danh sách
            }
            return View(supplier);
        }
        [HttpGet]
        public IActionResult XoaNhaCC(int id)
        {
            var supplier = _context.Suppliers.Find(id);
            if (supplier == null)
            {
                return NotFound();
            }
            return View(supplier);
        }

        [HttpPost, ActionName("XoaNhaCC")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var supplier = _context.Suppliers.Find(id);
            if (supplier != null)
            {
                _context.Suppliers.Remove(supplier);  // Xóa nhà cung cấp
                _context.SaveChanges();               // Lưu thay đổi vào DB
            }
            return RedirectToAction("QuanLyNhaCC"); // Quay lại danh sách
        }

    }
}
