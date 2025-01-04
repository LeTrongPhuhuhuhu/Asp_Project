using _6TL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace _6TL.Controllers
{
	public class ContactController : Controller
	{
		private readonly Db6TLContext _context;

		// Constructor để inject DbContext
		public ContactController(Db6TLContext context)
		{
			_context = context;
		}

		// Hiển thị form liên hệ
		public IActionResult LienHe()
		{
			return View("~/Views/Home/LienHe.cshtml");
		}

		// Xử lý khi người dùng gửi form
		[HttpPost]
		public async Task<IActionResult> LienHe(Contact model)
		{
			if (ModelState.IsValid)
			{
				// Cập nhật ngày tạo
				model.CreatedDate = DateTime.Now;

				// Lưu dữ liệu vào cơ sở dữ liệu
				_context.Contacts.Add(model);
				await _context.SaveChangesAsync();

				// Thông báo thành công
				TempData["SuccessMessage"] = "Thông tin liên hệ đã được gửi thành công!";
				return RedirectToAction("LienHe");
			}

			// Nếu model không hợp lệ, hiển thị lại form
			TempData["ErrorMessage"] = "Vui lòng kiểm tra lại thông tin!";
			return View(model);
		}
	}
}