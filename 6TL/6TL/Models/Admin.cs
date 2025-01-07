using System;
using System.Collections.Generic;

namespace _6TL.Models;

public partial class Admin
{
    public int AdminId { get; set; }

    public string? AdminName { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public string? Status { get; set; }

    public int? RoleId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Role? Role { get; set; }
}
