﻿using Microsoft.AspNetCore.Mvc;
using prjCatChaOnlineShop.Models;
using prjCatChaOnlineShop.Models.CDictionary;
using prjCatChaOnlineShop.Models.CModels;
using System.Text.Json;


namespace prjCatChaOnlineShop.Controllers.Api
{

    public class CouponController : Controller
    {
        public readonly cachaContext _context;
        public readonly CheckoutService _checkoutService;
        public CouponController(cachaContext context, CheckoutService checkoutService)
        {
            _context = context;
            _checkoutService = checkoutService;
        }


        public IActionResult CalculateDiscounts(bool useLoyaltyPoints, decimal couponSpecialOffer)
        {
            //從Session中獲得購物車的內容
            String Json = HttpContext.Session.GetString(CDictionary.SK_PURCHASED_PRODUCTS_LIST);
            //反序列化購物車內容
            List<CCartItem> cartItem = JsonSerializer.Deserialize<List<CCartItem>>(Json);

            // 計算使用優惠券折扣後的金額
            decimal totalPrice = 0;
            decimal couponBonus = 0;

            if (couponSpecialOffer != 0)
            {
                //計算使用優惠券折扣後的金額，並且只取整數
                totalPrice = Math.Floor((decimal)(cartItem.Sum(item => item.c小計) * couponSpecialOffer));
                //計算使用優惠券折扣了多少錢
                couponBonus = Math.Floor((decimal)(cartItem.Sum(item => item.c小計) - totalPrice));
            }
            // 使用 GetLoyaltyPoints 方法獲取初始的可使用紅利資訊
            CGetCouponPrice loyaltyPointsInfo = _checkoutService.GetLoyaltyPoints();


            //計算使用紅利的使用金額
            int loyaltyPointsDiscount = 0;
            // 使用者選擇使用紅利
            if (useLoyaltyPoints == true)
            {
                loyaltyPointsDiscount = (int)loyaltyPointsInfo.finalPrice;
            }

            //計算運費
            decimal shippingfee = 0;
            if ((int)(cartItem.Sum(item => item.c小計) - couponBonus - loyaltyPointsDiscount) < 2000)
            {
                shippingfee = 60;
            }

            //計算最終的總金額
            int finalTotalPrice = ((int)(cartItem.Sum(item => item.c小計) - couponBonus - loyaltyPointsDiscount + shippingfee));


            //創建一個匿名對象，商品更新後的總金額
            var response = new
            {
                loyaltypoints = loyaltyPointsDiscount,
                couponBonus = couponBonus,
                shippingfee=shippingfee,
                finalTotalPrice = finalTotalPrice,
            };
            return new JsonResult(response);

        }

    }
}

