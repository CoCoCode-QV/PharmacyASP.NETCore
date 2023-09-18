using System;
using System.Collections.Generic;

namespace Pharmacy.Data;

public partial class Product
{
    public int ProductId { get; set; }

    public string? ProductName { get; set; }

    public double ProductPrice { get; set; }

    public string? ProductDetail { get; set; }

    public string? ProductImage { get; set; }

    public double ProductInventory { get; set; }

    public int CategoryId { get; set; }

    public int SupplierId { get; set; }

    public int DiscountId { get; set; }

    public DateTime? ProductExpiryDate { get; set; }

    public string? ProductIngredients { get; set; }

    public bool? ProductPrescription { get; set; }

    public bool? ProductActive { get; set; }

    public virtual ICollection<CartDetail> CartDetails { get; set; } = new List<CartDetail>();

    public virtual Category Category { get; set; } = null!;

    public virtual Discount Discount { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual Supplier Supplier { get; set; } = null!;
}
