
using _6TL.Models;
using Microsoft.AspNetCore.Mvc;

namespace _6TL.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class ContactsController : Controller
	{
		private readonly Db6TLContext _context;

		public ContactsController(Db6TLContext context)
		{
			_context = context;
		}

		// Hiển thị danh sách liên hệ
		public IActionResult QuanLyLienHe()
		{
			var contacts = _context.Contacts.ToList();
			return View(contacts);
		}
		[HttpGet]
		public IActionResult Filter(string status, string date)
		{
			var query = _context.Contacts.AsQueryable();

			// Lọc theo trạng thái
			if (!string.IsNullOrEmpty(status))
			{
				if (status == "recent")
				{
					query = query.OrderByDescending(c => c.CreatedDate).Take(10); // Lấy 10 liên hệ gần đây nhất
				}
				else if (status == "pending")
				{
					query = query.Where(c => c.Status == "Chưa xử lý");
				}
				else if (status == "completed")
				{
					query = query.Where(c => c.Status == "Đã xử lý");
				}
			}

			// Lọc theo ngày
			if (!string.IsNullOrEmpty(date) && DateTime.TryParse(date, out DateTime filterDate))
			{
				query = query.Where(c => c.CreatedDate.Date == filterDate.Date);
			}

			// Trả về kết quả dưới dạng JSON
			var filteredContacts = query
				.Select(c => new
				{
					ContactId = c.ContactId,
					Name = c.Name,
					Title = c.Title,
					Email = c.Email,
					Phone = c.Phone,
					CreatedDate = c.CreatedDate.ToString("yyyy-MM-dd"),
					Status = c.Status
				})
				.ToList();

			return Json(new { success = true, data = filteredContacts });
		}
		[HttpGet]
		public IActionResult Search(string keyword)
		{
			var contacts = _context.Contacts
				.Where(c => c.Name.Contains(keyword) || c.Title.Contains(keyword)||c.Email.Contains(keyword)||c.Phone.Contains(keyword))
				.Select(c => new
				{
					ContactId = c.ContactId,
					Name = c.Name,
					Title = c.Title,
					Email= c.Email,
					Phone =c.Phone,
					CreatedDate = c.CreatedDate.ToString("yyyy-MM-dd"),
					Status = c.Status
				})
				.ToList();

			return Json(new { success = true, data = contacts });
		}
		[HttpGet]
		//lấy dữ liệu database để hiện lên 1 contact trên popup
		public JsonResult GetContactDetails(int id)
		{
			Console.WriteLine($"Received ID: {id}");
			var contact = _context.Contacts
				.Where(c => c.ContactId == id)
				.FirstOrDefault();

			if (contact != null)
			{
				// Trả về dữ liệu dưới dạng JSON
				var contactDetails = new
				{
					ContactId = contact.ContactId,
					Name = contact.Name,
					Title = contact.Title,
					Email = contact.Email,
					Phone = contact.Phone,
					CreatedDate = contact.CreatedDate.ToString("yyyy-MM-dd"),
					Content = contact.Message,
					Status = contact.Status
				};

				return Json(contactDetails);
			}

			return Json(null);
		}
		// Cập nhật trạng thái liên hệ
		[HttpPost]
		public IActionResult UpdateStatus(int contactId, string status)
		{
			var contact = _context.Contacts.FirstOrDefault(c => c.ContactId == contactId);

			if (contact != null)
			{
				contact.Status = status; // Cập nhật trạng thái
				_context.SaveChanges(); // Lưu thay đổi vào database

				return Json(new { success = true, message = "Trạng thái đã được cập nhật!" });
			}

			return Json(new { success = false, message = "Không tìm thấy liên hệ với ID này!" });
		}

		// Xóa liên hệ
		[HttpPost]
		public IActionResult Delete(int id)
		{
			var contact = _context.Contacts.Find(id);
			if (contact == null)
			{
				return Json(new { success = false, message = "Liên hệ không tồn tại." });
			}

			_context.Contacts.Remove(contact);
			_context.SaveChanges();

			return Json(new { success = true, message = "Xóa liên hệ thành công." });
		}
	}
}