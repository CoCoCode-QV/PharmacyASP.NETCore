using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;



public partial class Discount
{
    public int DiscountId { get; set; }

    [Display(Name = "Phần trăm giảm giá")]
    //[Required(ErrorMessage = "Phần trăm giảm giá là trường bắt buộc")]
    [Range(0, 100, ErrorMessage = "Phần trăm giảm giá phải nằm trong khoảng từ 0 đến 100")]
    [DefaultValue(0)]
    public double? DiscountPercent { get; set; }

    [Display(Name = "Mã giảm giá")]
    [Required(ErrorMessage = "Mã giảm giá là trường bắt buộc")]
    public string? DiscountName { get; set; }

    [Display(Name = "Chi tiết giảm giá")]
    //[Required(ErrorMessage = "Chi tiết giảm giá là trường bắt buộc")]
    public string? DiscountDetail { get; set; }

    public virtual ICollection<ProductDiscount> ProductDiscounts { get; set; } = new List<ProductDiscount>();
}
