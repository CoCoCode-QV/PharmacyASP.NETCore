using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Pharmacy.Data;

public partial class Product
{
    
    public int ProductId { get; set; }

    [Required(ErrorMessage ="Trường này không được để trống!")]
    [Display(Name ="Tên sản phẩm")]
    public string? ProductName { get; set; }

    [Display(Name = "Giá")]
    [DefaultValue(0)]
    [Required(ErrorMessage = "Trường này không được để trống!")]
    public double ProductPrice { get; set; }

    [Required(ErrorMessage = "Trường này không được để trống!")]
    [Display(Name = "Chi tiết sản phẩm")]
    public string? ProductDetail { get; set; }

   
    [Display(Name = "Hình ảnh")]
    public string? ProductImage { get; set; }

    [Display(Name = "Kho")]
    [DefaultValue(0)]
    [Required(ErrorMessage = "Trường này không được để trống!")]
    public double ProductInventory { get; set; }

    [Display(Name = "Danh mục sản phẩm")]
    [Required(ErrorMessage = "Trường này không được để trống!")]
    public int CategoryId { get; set; }

    [Display(Name = "Nhà cung cấp")]
    [Required(ErrorMessage = "Trường này không được để trống!")]
    public int SupplierId { get; set; }

    [Display(Name = "Mã giảm giá")]
    [Required(ErrorMessage = "Trường này không được để trống!")]
    public int DiscountId { get; set; }

    [Display(Name = "Ngày hết hạn")]
    [DataType(DataType.DateTime)]
    [Required(ErrorMessage = "Ngày hết hạn là trường bắt buộc")]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}", ApplyFormatInEditMode = true)]
    public DateTime? ProductExpiryDate { get; set; }

    [Display(Name = "Thành phần")]
    [Required(ErrorMessage = "Thành phần là trường bắt buộc")]
    public string? ProductIngredients { get; set; }


    [Display(Name = "Đơn thuốc")]
    [Required(ErrorMessage = "Đơn thuốc là trường bắt buộc")]
    [DefaultValue(false)]
    public bool? ProductPrescription { get; set; }

    [Display(Name = "Trạng thái")]
    [Required(ErrorMessage = "Trạng thái là trường bắt buộc")]
    [DefaultValue(true)]
    public bool? ProductActive { get; set; }

    public virtual ICollection<CartDetail> CartDetails { get; set; } = new List<CartDetail>();

    public virtual Category Category { get; set; } = null!;

    public virtual Discount Discount { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual Supplier Supplier { get; set; } = null!;
}
