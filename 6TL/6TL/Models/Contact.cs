using System;
using System.Collections.Generic;

namespace _6TL.Models;

public partial class Contact
{
    public int ContactId { get; set; }

    public string Title { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public string Message { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime CreatedDate { get; set; }
}
