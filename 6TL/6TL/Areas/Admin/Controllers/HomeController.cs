using System;
using _6TL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _6TL.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
	{
        private readonly Db6TLContext _context;

        public HomeController(Db6TLContext context)
        {
            _context = context;
        }

        public IActionResult Index()
		{
			return View();
		}
        public IActionResult ThemBlog()
        {
            return View();
        }
        public IActionResult QuanLyBlog()
        {
            var blogs = _context.Blogs.ToList();
            return View(blogs);
        }
        [HttpPost]
        [Route("add")]
        public IActionResult Add(Blog blog, IFormFile imageFile)
        {
            if (imageFile != null)
            {
                string filePath = Path.Combine("wwwroot/img", imageFile.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    imageFile.CopyTo(stream);
                }
                blog.HinhAnh = "/img/" + imageFile.FileName;
            }

            _context.Blogs.Add(blog);
            _context.SaveChanges();
            return RedirectToAction("QuanLyBlog");
        }
        [HttpPost]
        [Route("edit")]
        public IActionResult Edit(Blog blog, IFormFile imageFile)
        {
            var existingBlog = _context.Blogs.Find(blog.BlogId);
            if (existingBlog != null)
            {
                existingBlog.TieuDe = blog.TieuDe;
                existingBlog.NoiDung = blog.NoiDung;

                if (imageFile != null)
                {
                    string filePath = Path.Combine("wwwroot/img", imageFile.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        imageFile.CopyTo(stream);
                    }
                    existingBlog.HinhAnh = "/img/" + imageFile.FileName;
                }

                _context.SaveChanges();
            }

            return RedirectToAction("QuanLyBlog");
        }
        [HttpPost]
        [Route("delete/{id}")]
        public IActionResult Delete(int id)
        {
            var blog = _context.Blogs.Find(id);
            if (blog != null)
            {
                _context.Blogs.Remove(blog);
                _context.SaveChanges();
            }
            return RedirectToAction("QuanLYBlog");
        }
        public IActionResult GioiThieu()
		{
			return View();
		}
        public IActionResult QuanLyDanhMuc() 
        {
            return View();
        }
        public IActionResult QuanLyDonHang()
        {
            return View();
        }
        public IActionResult QuanLyChiTietDonHang()
        {
            return View();
        }
        public IActionResult QuanLyAdmin()
        {
            return View();
        }
        public IActionResult QuanLyUser()
        {
            return View();
        }
        public IActionResult QuanLyLienHe()
        {
            return View();
        }
        public IActionResult QuanLyDanhGia()
        {
            return View();
        }
    }
}
