using System;
using System.Collections.Generic;



public partial class OrderDetail
{
    public int OrderDetails { get; set; }

    public int? OrderDetailsQuantity { get; set; }

    public double? OrderDetailsPrice { get; set; }

    public int OrderId { get; set; }

    public double? OrderDetailsTemporaryPrice { get; set; }

    public int? CostId { get; set; }

    public int? OrderDiscountId { get; set; }

    public virtual ProductCost? Cost { get; set; }

    public virtual Order Order { get; set; } = null!;
}
