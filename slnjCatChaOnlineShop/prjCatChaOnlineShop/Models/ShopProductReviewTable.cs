﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace prjCatChaOnlineShop.Models;

public partial class ShopProductReviewTable
{
    public int? MemberId { get; set; }

    public int? ProductId { get; set; }

    public string ReviewContent { get; set; }

    public decimal? ProductRating { get; set; }

    public int ProductReviewId { get; set; }

    public DateTime? ReviewTime { get; set; }

    public bool? HideReview { get; set; }

    public int? OrderId { get; set; }

    public virtual ShopMemberInfo Member { get; set; }

    public virtual ShopOrderTotalTable Order { get; set; }

    public virtual ShopProductTotal Product { get; set; }
}