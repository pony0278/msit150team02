﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace prjCatChaOnlineShop.Models;

public partial class ShopMyCatNameList
{
    public int MyCatList { get; set; }

    public int MemberId { get; set; }

    public virtual ShopMemberInfo Member { get; set; }

    public virtual ICollection<ShopCatStatus> ShopCatStatus { get; set; } = new List<ShopCatStatus>();

    public virtual ICollection<ShopMemberInfo> ShopMemberInfo { get; set; } = new List<ShopMemberInfo>();
}