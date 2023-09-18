using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Pharmacy.Data;

public partial class Account 
{
    public int AccountId { get; set; }

    public string DisplayName { get; set; } = null!;

    public string? UserName { get; set; }

    public string? Password { get; set; }

    public int Type { get; set; }
}
