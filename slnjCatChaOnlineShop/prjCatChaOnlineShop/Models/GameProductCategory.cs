﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace prjCatChaOnlineShop.Models;

public partial class GameProductCategory
{
    public int ProductCategoryId { get; set; }

    public string CategoryName { get; set; }

    public virtual ICollection<GameProductTotal> GameProductTotal { get; set; } = new List<GameProductTotal>();
}