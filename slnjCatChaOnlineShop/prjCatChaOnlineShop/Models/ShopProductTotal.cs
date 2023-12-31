﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace prjCatChaOnlineShop.Models;

public partial class ShopProductTotal
{
    public string ProductName { get; set; }

    public int ProductId { get; set; }

    public int? ProductCategoryId { get; set; }

    public string ProductDescription { get; set; }

    public decimal? ProductPrice { get; set; }

    public string Size { get; set; }

    public string Weight { get; set; }

    public DateTime? ReleaseDate { get; set; }

    public bool? Discontinued { get; set; }

    public int? RemainingQuantity { get; set; }

    public int? SupplierId { get; set; }

    public DateTime? OffDay { get; set; }

    public string Attributes { get; set; }

    public decimal? Discount { get; set; }

    public bool? PushToShop { get; set; }

    public string ProductImage1 { get; set; }

    public string ProductImage2 { get; set; }

    public string ProductImage3 { get; set; }

    public int? ProductSpId { get; set; }

    public virtual ShopProductCategory ProductCategory { get; set; }

    public virtual ShopProductSpecification ProductSp { get; set; }

    public virtual ICollection<ShopFavoriteDataTable> ShopFavoriteDataTable { get; set; } = new List<ShopFavoriteDataTable>();

    public virtual ICollection<ShopOrderDetailTable> ShopOrderDetailTable { get; set; } = new List<ShopOrderDetailTable>();

    public virtual ICollection<ShopProductImageTable> ShopProductImageTable { get; set; } = new List<ShopProductImageTable>();

    public virtual ICollection<ShopProductReviewTable> ShopProductReviewTable { get; set; } = new List<ShopProductReviewTable>();

    public virtual ICollection<ShopProductSpecification> ShopProductSpecification { get; set; } = new List<ShopProductSpecification>();

    public virtual ICollection<ShopRoom> ShopRoom { get; set; } = new List<ShopRoom>();

    public virtual ShopProductSupplier Supplier { get; set; }
}