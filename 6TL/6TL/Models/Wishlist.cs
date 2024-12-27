using System;
using System.Collections.Generic;

namespace _6TL.Models;

public partial class Wishlist
{
    public int WishlistId { get; set; }

    public int? CustomerId { get; set; }

    public int? ProductId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual Product? Product { get; set; }
}
