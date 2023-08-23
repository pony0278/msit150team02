using prjCatChaOnlineShop.Models.ViewModels;
using System.Reflection;
using prjCatChaOnlineShop.Models;
using prjCatChaOnlineShop.Models.CDictionary;
using System.Text.Json;

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
                            })
                        .Where(ct => ct.Usable == true)
                        .ToList();

                        return usableCoupons;
                    }
                }
            }

            return new List<CGetUsableCouponModel>();
        }


    }
}
