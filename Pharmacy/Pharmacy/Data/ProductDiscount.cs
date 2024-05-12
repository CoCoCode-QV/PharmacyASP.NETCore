using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;



public partial class ProductDiscount
{
    public int ProductDiscountId { get; set; }

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

    public int DiscountId { get; set; }

    public int CostId { get; set; }

    public virtual ProductCost? Cost { get; set; }

    public virtual Discount? Discount { get; set; }
}
