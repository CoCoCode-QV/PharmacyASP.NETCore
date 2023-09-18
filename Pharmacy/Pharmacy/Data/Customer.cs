﻿using System;
using System.Collections.Generic;

namespace Pharmacy.Data;

public partial class Customer
{
    public int CustomerId { get; set; }

    public string? CustomerName { get; set; }

    public string? CustomerPhone { get; set; }

    public string? CustomerAddress { get; set; }

    public string? CustomerEmail { get; set; }

    public int? CustomerSex { get; set; }

    public int? CustomerAge { get; set; }

    public string? CustomerAllergies { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
