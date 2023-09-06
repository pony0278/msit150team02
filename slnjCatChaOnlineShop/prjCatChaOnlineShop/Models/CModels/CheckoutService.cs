using prjCatChaOnlineShop.Models.ViewModels;
using System.Reflection;
using prjCatChaOnlineShop.Models;
using prjCatChaOnlineShop.Models.CDictionary;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace prjCatChaOnlineShop.Models.CModels
{
    public class CheckoutService
    {
        private readonly cachaContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CheckoutService(cachaContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        //找到會員ID
        private int? GetCurrentMemberId()
        {
            var memberInfoJson = _httpContextAccessor.HttpContext?.Session.GetString(CDictionary.CDictionary.SK_LOINGED_USER);
            if (memberInfoJson != null)
            {
                var memberInfo = JsonSerializer.Deserialize<ShopMemberInfo>(memberInfoJson);
                return memberInfo.MemberId;
            }

            return null;
        }


        public List<CGetUsableCouponModel> GetUsableCoupons(int Id)
        {
            // 使用 Entity Framework Core 和 LINQ 查詢可用的優惠券
            var memberId = GetCurrentMemberId();
            if (memberId != null)
            {
                using (var dbContext = new cachaContext()) // 使用新的DbContext
                {
                    if (_context != null)
                    {
                        var usableCoupons = _context.ShopMemberCouponData
                        .Where(mcd => mcd.MemberId == memberId && mcd.CouponStatusId == false)
                        .Join(
                            _context.ShopCouponTotal,
                            mcd => mcd.CouponId,
                            ct => ct.CouponId,
                            (mcd, ct) => new CGetUsableCouponModel
                            {
                                CouponID = ct.CouponId,
                                CouponName = ct.CouponName,
                                CouponContent = ct.CouponContent,
                                ExpiryDate = (DateTime)ct.ExpiryDate,
                                Usable = ct.Usable,
                                SpecialOffer = (decimal)ct.SpecialOffer,
                            })
                        .Where(ct => ct.Usable == true && ct.ExpiryDate >= DateTime.Now)
                        .GroupBy(ct=>ct.CouponID) // 根據 CouponID 分組
                        .Select(group=>group.First())// 選擇每個組中的第一張優惠券
                        .ToList();

                        return usableCoupons;
                    }
                }
            }

            return new List<CGetUsableCouponModel>();
        }

        //查詢會員可以使用的常用地址
        public List<CgetUsableAddressModel> GetUsableAddresses(int Id)
        {
            // 使用 Entity Framework Core 和 LINQ 查詢可用的地址
            var memberId = GetCurrentMemberId();
            if (memberId != null)
            {
                using (var dbContext = new cachaContext())// 使用新的DbContext，離開using會自動關連接
                {
                    if (_context != null)
                    {
                        var usableAddress = _context.ShopCommonAddressData
                                .Where(cad => cad.MemberId == memberId)
                                .Select(cad => new CgetUsableAddressModel
                                {
                                    AddressID = cad.AddressId,
                                    RecipientName = cad.RecipientName,
                                    RecipientAddress = cad.RecipientAddress,
                                    RecipientPhone = cad.RecipientPhone,
                                }).ToList();
                        return usableAddress;
                    }
                }
            }
            return new List<CgetUsableAddressModel>(); // 如果沒有找到可用地址，返回空列表
        }

        //查詢折抵紅利的金額
        public CGetCouponPrice GetLoyaltyPoints()
        {
           CGetCouponPrice getCouponPrice= new CGetCouponPrice();


            //先從session中取得初始的紅利點數
            string couponjson = _httpContextAccessor.HttpContext?.Session.GetString(CDictionary.CDictionary.SK_LOINGED_USER);
            var memberInfo = JsonSerializer.Deserialize<ShopMemberInfo>(couponjson);

            //先從session中找到購物車裡的商品
            string cartjson = _httpContextAccessor.HttpContext?.Session.GetString(CDictionary.CDictionary.SK_PURCHASED_PRODUCTS_LIST);
            var cart = JsonSerializer.Deserialize<List<CCartItem>>(cartjson);

            if (memberInfo != null && cart != null)
            {
                //會員初始的紅利點數
                var loyaltyPoints = memberInfo.LoyaltyPoints;
                //計算現有的紅利等於多少元
                var transferPoint = (memberInfo.LoyaltyPoints) / 100;
                //會員初始的購物車金額
                var price = cart.Sum(item => item.c小計);
                //計算購物車初始金額可以折抵多少金額(單筆訂單10%)
                var tenPercentPrice = price * 0.1;
                //計算折抵金額等於多少紅利點數
                var priceToLoalty = tenPercentPrice * 100;
                
                //折抵規則
                if (loyaltyPoints < priceToLoalty)  //如果現有紅利不足以折抵該筆訂單10%，最後可折抵就是以現有紅利為主
                {
                    getCouponPrice.finalBonus = (int)loyaltyPoints;
                    getCouponPrice.finalPrice = (decimal)transferPoint;
                }
                else //如果現有紅利足夠折抵該筆訂單10%，最後可折抵以該筆訂單為主
                {
                    getCouponPrice.finalBonus = (int)priceToLoalty;
                    getCouponPrice.finalPrice = (decimal)tenPercentPrice;
                }
            }
            return getCouponPrice;

        }
    }
}
