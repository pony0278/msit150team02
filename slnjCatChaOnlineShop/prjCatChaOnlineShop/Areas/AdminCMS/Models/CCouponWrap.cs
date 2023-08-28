﻿namespace prjCatChaOnlineShop.Areas.AdminCMS.Models
{
    public class CCouponWrap
    {
        public int CouponId { get; set; }

        public string CouponName { get; set; }

        public string CouponContent { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public int? TotalQuantity { get; set; }

        public bool? Usable { get; set; }
    }
}
