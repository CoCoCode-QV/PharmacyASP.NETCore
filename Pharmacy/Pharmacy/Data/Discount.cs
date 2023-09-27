using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Pharmacy.Data;

public partial class Discount
{
    public int DiscountId { get; set; }

    [Display(Name ="Phần trăm giảm giá")]
    [Required(ErrorMessage = "Phần trăm giảm giá là trường bắt buộc")]
    [Range(0, 100, ErrorMessage = "Phần trăm giảm giá phải nằm trong khoảng từ 0 đến 100")]
    [DefaultValue(0)]
    public double? DiscountPercent { get; set; }

    [Display(Name = "Mã giảm giá")]
    [Required(ErrorMessage = "Mã giảm giá là trường bắt buộc")]
    public string? DiscountName { get; set; }

    [Display(Name = "Ngày bắt đầu giảm giá")]
    [DataType(DataType.DateTime)]
    [Required(ErrorMessage = "Ngày bắt đầu giảm giá là trường bắt buộc")]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}", ApplyFormatInEditMode = true)]
    public DateTime? DiscountStartDate { get; set; }

    [Display(Name = "Ngày kết thúc giảm giá")]
    [DataType(DataType.DateTime)]
    [Required(ErrorMessage = "Ngày kết thúc giảm giá là trường bắt buộc")]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}", ApplyFormatInEditMode = true)]
    public DateTime? DiscountEndDate { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
