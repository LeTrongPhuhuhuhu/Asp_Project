using System;
using System.Collections.Generic;

namespace _6TL.Models;

public partial class Blog
{
    public int BlogId { get; set; }

    public string TieuDe { get; set; } = null!;

    public string NoiDung { get; set; } = null!;

    public string? HinhAnh { get; set; }

    public DateTime? CreatedAt { get; set; }
}
