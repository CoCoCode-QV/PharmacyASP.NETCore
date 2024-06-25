using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;



public partial class ProductCost
{
    public int CostId { get; set; }

    public int ProductId { get; set; }

    public int SupplierId { get; set; }

    public double CostPrice { get; set; }

    //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime? ReceivingDate { get; set; }

    public bool CostActive { get; set; }

    public double ProductInventory { get; set; }

    //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime? ProductExpiryDate { get; set; }

    public virtual ICollection<CartDetail> CartDetails { get; set; } = new List<CartDetail>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

  

    public virtual Product Product { get; set; } = null!;

    public virtual ICollection<ProductDiscount> ProductDiscounts { get; set; } = new List<ProductDiscount>();

    public virtual Supplier Supplier { get; set; } = null!;
}
