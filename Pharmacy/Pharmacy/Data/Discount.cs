using System;
using System.Collections.Generic;

namespace Pharmacy.Data;

public partial class Discount
{
    public int DiscountId { get; set; }

    public double? DiscountPercent { get; set; }

    public string? DiscountName { get; set; }

    public DateTime? DiscountStartDate { get; set; }

    public DateTime? DiscountEndDate { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
