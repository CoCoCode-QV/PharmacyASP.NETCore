using System;
using System.Collections.Generic;

namespace Pharmacy.Data;

public partial class Order
{
    public int OrderId { get; set; }

    public DateTime OrderDate { get; set; }

    public string? OrderAddress { get; set; }

    public int OrderStatus { get; set; }

    public int CustomerId { get; set; }

    public bool OrderAccept {  get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
