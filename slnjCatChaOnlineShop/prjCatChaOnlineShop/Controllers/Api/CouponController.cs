using Microsoft.AspNetCore.Mvc;
using prjCatChaOnlineShop.Models;
using prjCatChaOnlineShop.Models.CDictionary;
using prjCatChaOnlineShop.Models.CModels;
using System.Text.Json;

namespace prjCatChaOnlineShop.Controllers.Api
{
   
    public class CouponController : Controller
    {
        public readonly cachaContext _context;

        public CouponController(cachaContext context)
        {
            _context = context;
        }

        //如果有使用優惠券則在此計算金額
        public IActionResult couponDiscount(decimal SpecialOffer)
        {
            //從Session中獲得購物車的內容
            String Json = HttpContext.Session.GetString(CDictionary.SK_PURCHASED_PRODUCTS_LIST);
            //反序列化購物車內容
            List<CCartItem> cartItem = JsonSerializer.Deserialize<List<CCartItem>>(Json);
            //計算折扣後的金額，並且只取整數
            decimal totalPrice = Math.Floor((decimal)(cartItem.Sum(item => item.c小計) * SpecialOffer));
            //計算使用優惠券折扣了多少
            decimal couponBonus = Math.Floor((decimal)(cartItem.Sum(item => item.c小計) - totalPrice));

            //創建一個匿名對象，商品更新後的總金額
            var response = new
            {
                totalPrice = totalPrice,
                couponBonus= couponBonus,
            };
            return new JsonResult(response);
        }

    }
}

