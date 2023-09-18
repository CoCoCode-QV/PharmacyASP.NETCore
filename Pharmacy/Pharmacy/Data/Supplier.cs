using System;
using System.Collections.Generic;

namespace Pharmacy.Data;

public partial class Supplier
{
    public int SupplierId { get; set; }

    public string? SupplierName { get; set; }

    public string? SupplierEmail { get; set; }

    public string? SupplierPhone { get; set; }

    public string? SupplierAddress { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
