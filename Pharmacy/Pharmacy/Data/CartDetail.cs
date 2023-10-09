using System;
using System.Collections.Generic;

namespace Pharmacy.Data;

public partial class CartDetail
{
    public int CartDetailId { get; set; }

    public int? CartDetailQuantity { get; set; }

    public double? CartDetailTemporaryPrice { get; set; }

    public double? CartDetailPriceCurrent { get; set; }

    public int CartId { get; set; }

    public int ProductId { get; set; }

    public virtual Cart Cart { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
