using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;



public partial class Product
{
    public int ProductId { get; set; }

    [Required(ErrorMessage = "Trường này không được để trống!")]
    [Display(Name = "Tên sản phẩm")]
    public string? ProductName { get; set; }

    [Required(ErrorMessage = "Trường này không được để trống!")]
    [Display(Name = "Chi tiết sản phẩm")]
    public string? ProductDetail { get; set; }

    [Display(Name = "Hình ảnh")]
    public string? ProductImage { get; set; }

    [Display(Name = "Danh mục sản phẩm")]
    [Required(ErrorMessage = "Trường này không được để trống!")]
    public int CategoryId { get; set; }

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

    [Display(Name = "Giá")]
    [DefaultValue(0)]
    [Required(ErrorMessage = "Trường này không được để trống!")]
    public double? ProductPrice { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<ProductCost> ProductCosts { get; set; } = new List<ProductCost>();
}
