﻿using System;
using System.Collections.Generic;

namespace _6TL.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string? ProductName { get; set; }

    public string Slug { get; set; } = null!;

    public string? Image { get; set; }

    public string? Material { get; set; }

    public string? Color { get; set; }

    public string? Brand { get; set; }

    public int? WarrantyPeriod { get; set; }

    public string? ProductDescription { get; set; }

    public int? Quantity { get; set; }

    public decimal? Price { get; set; }

    public int? DiscountId { get; set; }

    public int? SupplierId { get; set; }

    public int? CategoryId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Category? Category { get; set; }

    public virtual Discount? Discount { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual Supplier? Supplier { get; set; }

    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}
