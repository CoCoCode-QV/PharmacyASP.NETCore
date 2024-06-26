﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public partial class Category
{
    public int CategoryId { get; set; }
    [Display(Name = "Tên Danh mục: ")]
    [Required(ErrorMessage = "Tên danh mục không được để trống")]
    public string CategoryName { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
