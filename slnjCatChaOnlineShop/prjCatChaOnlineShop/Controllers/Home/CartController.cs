using Microsoft.AspNetCore.Mvc;
using prjCatChaOnlineShop.Models.CDictionary;
using prjCatChaOnlineShop.Models;
using prjCatChaOnlineShop.Models.CModels;
using prjCatChaOnlineShop.Models.ViewModels;
using System.Text.Json;

namespace prjCatChaOnlineShop.Controllers.Home
{
    public class CartController : Controller
    {

        private readonly CheckoutService _checkoutService;

        public CartController(CheckoutService checkoutService)
        {
            _checkoutService = checkoutService;
        }

        public IActionResult ConfrimOrder()
        { 
            return View();
        }
        public IActionResult Checkout()
        {
            if (HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER) != null)
            {
                // 從 Session 中讀取抓到的 MEMBER ID
                var memberInfoJson = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER);
                var memberInfo = JsonSerializer.Deserialize<ShopMemberInfo>(memberInfoJson);

                // 使用 CheckoutService 來獲取可用的優惠券
                var usableCoupons = _checkoutService.GetUsableCoupons(memberInfo.MemberId);

                var viewModel = new CCheckoutViewModel
                {
                    memberUsableCoupon = usableCoupons ?? new List<CGetUsableCouponModel>(), // 初始化為    空列表
                };

                return View(viewModel);
            }

            return View();
        }
        public IActionResult Cart()
        {
            return View();
        }
    }
}
