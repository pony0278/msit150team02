﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace prjCatChaOnlineShop.Models;

public partial class NewsletterTemplate
{
    public int TemplateId { get; set; }

    public string HeaderImage { get; set; }

    public string GooterImage { get; set; }

    public virtual Newsletter Newsletter { get; set; }
}