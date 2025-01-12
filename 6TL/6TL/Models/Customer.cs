using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace _6TL.Models;

public partial class Customer
{
	public int CustomerId { get; set; }

	[Required(ErrorMessage = "Chưa điền Họ và Tên !")]
	public string FullName { get; set; }

	[Required(ErrorMessage = "Chưa điền Email !")]
	[EmailAddress(ErrorMessage = "Email không hợp lệ !")]
	public string Email { get; set; }

	[Required(ErrorMessage = "Chưa điền địa chỉ !")]
	public string Address { get; set; }

	[Required(ErrorMessage = "Chưa điền số điện thoại !")]
	[RegularExpression(@"^\d+$", ErrorMessage = "Số điện thoại là các chữ số !")]
	public string PhoneNumber { get; set; }

	[Required(ErrorMessage = "Chưa điền mật khẩu !")]
	[RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)[A-Za-z\d]{6,}$",
		ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự, bao gồm chữ cái, chữ số và ít nhất một chữ in hoa")]
	public string Password { get; set; }
	public bool? IsEmailConfirmed { get; set; } = false; // Trạng thái xác nhận email
	public string? EmailConfirmationToken { get; set; } // Token để xác nhận email
	public string? PasswordResetToken { get; set; } // Token để xác nhận đổi password
	public DateTime? PasswordResetExpires { get; set; } // Biến lưu trữ thời gian hết hạn token

	public int RoleId { get; set; }

	public DateTime? CreatedAt { get; set; }

	public DateTime? UpdatedAt { get; set; }

	public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

	public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

	public virtual Role? Role { get; set; }

	public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}