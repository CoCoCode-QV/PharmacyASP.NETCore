using System;
using System.Collections.Generic;

namespace Pharmacy.Data;

public partial class OrderDetail
{
    public int OrderDetails { get; set; }

    public int? OrderDetailsQuantity { get; set; }

    public double? OrderDetailsPrice { get; set; }

    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
