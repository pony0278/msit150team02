﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace prjCatChaOnlineShop.Models;

public partial class GameBanUser
{
    public int Id { get; set; }

    public int? MemberId { get; set; }

    public DateTime? BannedTime { get; set; }

    public DateTime? UnBannedTime { get; set; }
}