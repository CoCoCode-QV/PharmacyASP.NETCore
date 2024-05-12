using System;
using System.Collections.Generic;


public partial class CartDetail
{
    public int CartDetailId { get; set; }

    public int CartDetailQuantity { get; set; }

    public double? CartDetailTemporaryPrice { get; set; }

    public int CartId { get; set; }

    public double? CartDetailPriceCurrent { get; set; }

    public int CostId { get; set; }

    public virtual Cart Cart { get; set; } = null!;

    public virtual ProductCost? Cost { get; set; }
}
