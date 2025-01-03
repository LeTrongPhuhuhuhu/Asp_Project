using System;
using System.Collections.Generic;

using System.ComponentModel.DataAnnotations.Schema;


namespace _6TL.Models;

public partial class Color
{
    public int ColorId { get; set; }

    public string ColorName { get; set; } = null!;

    [NotMapped]
    public string ColorCode { get; set; }
    public virtual ICollection<ProductColor> ProductColors { get; set; } = new List<ProductColor>();

}
