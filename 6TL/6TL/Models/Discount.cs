using System;
using System.Collections.Generic;

namespace _6TL.Models;

public partial class Discount
{
    public int DiscountId { get; set; }

    public string DiscountName { get; set; } = null!;

    public string DiscountType { get; set; } = null!;

    public decimal DiscountValue { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public decimal? MinOrderValue { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
