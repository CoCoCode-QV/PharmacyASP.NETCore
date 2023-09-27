using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Pharmacy.Data;

public partial class Supplier
{
    public int SupplierId { get; set; }

    [Required(ErrorMessage ="Không được để trống tên nhà cung cấp")]
    [Display(Name ="Tên nhà cung cấp")]
    public string? SupplierName { get; set; }

    [EmailAddress]
    [Display(Name ="Địa chỉ Email")]
    [Required(ErrorMessage ="Không được để trống địa chỉ Email")]
    public string? SupplierEmail { get; set; }

    [Phone]
    [Display(Name = "Số điện thoại")]
    [Required(ErrorMessage ="Không được để trống SĐT nhà cung cấp")]
    public string? SupplierPhone { get; set; }

    [Display(Name ="Địa chỉ nhà cung cấp")]
    [Required(ErrorMessage = "Không được để trống địa chỉ nhà cung cấp")]
    public string? SupplierAddress { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
