using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjCatChaOnlineShop.Models;
using prjCatChaOnlineShop.Models.CDictionary;
using prjCatChaOnlineShop.Models.CModels;
using System.Text.Json;


namespace prjCatChaOnlineShop.Controllers.Api
{
    [Route("api/Api/[controller]")]
    [ApiController]
    public class FeedCatGetCouponController: ControllerBase
    {
        private readonly cachaContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public FeedCatGetCouponController(IHttpContextAccessor httpContextAccessor, cachaContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }
        [HttpPost]
        public IActionResult updateUserCoupon(CPlayerItem c)
        {
            try
            {
                var memberInfoJson = _httpContextAccessor.HttpContext?.Session.GetString(CDictionary.SK_LOINGED_USER);
                var memberInfo = JsonSerializer.Deserialize<ShopMemberInfo>(memberInfoJson);
                int _memberId = memberInfo.MemberId;

                var data = _context.GameItemPurchaseRecord.FirstOrDefault(p => p.MemberId == _memberId && p.ProductId == c.fProductId);

                if (data != null)
                {
                    data.QuantityOfInGameItems++;
                    var dataInCouponTable = new ShopMemberCouponData
                    {
                        MemberId = _memberId,
                        CouponId = c.fCouponId,
                        CouponStatusId = false
                    };
                    _context.ShopMemberCouponData.Add(dataInCouponTable);
                    _context.SaveChanges();
                    return Ok(new { message = "已存入優惠券" });
                }
                return Ok(new { message = "API完成" });
            }
            catch (Exception ex)
            {
                // 處理異常情況
                return StatusCode(500, "發生錯誤：" + ex.Message);
            }
        }

    }
}
