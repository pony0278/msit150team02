﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace prjCatChaOnlineShop.Models;

public partial class GameItemPurchaseRecord
{
    public int? MemberId { get; set; }

    public int? ProductId { get; set; }

    public string? CharacterName { get; set; }

    public string PurchaseTime { get; set; }

    public string? ItemName { get; set; }

    public int GameItemPurchaseRecordId { get; set; }

    public int? QuantityOfInGameItems { get; set; }

    public virtual GameProductTotal Product { get; set; }
}