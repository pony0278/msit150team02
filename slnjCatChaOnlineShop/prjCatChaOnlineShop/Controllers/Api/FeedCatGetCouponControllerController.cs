using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjCatChaOnlineShop.Models;
using prjCatChaOnlineShop.Models.CModels;

namespace prjCatChaOnlineShop.Controllers.Api
{
    [Route("api/Api/[controller]")]
    [ApiController]
    public class FeedCatGetCouponController: ControllerBase
    {
        private readonly cachaContext _context;
        public FeedCatGetCouponController(cachaContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult updateUserCoupon(CPlayerItem c/*,CShopCouponMemberData cp*/)
        {
            var data = _context.GameItemPurchaseRecord.FirstOrDefault(p => p.MemberId == c.fId && p.ProductId == c.fProductId);
            //var dataInCouponTable = _context.ShopMemberCouponData.FirstOrDefault(p => p.MemberId == c.fId && p.CouponId == cp.fCouponId);
            if (data != null)//如果使用者之前有這個優惠券了
            {//TODO 這邊要加入同步寫入shop coupon member data
                data.MemberId = c.fId;
                data.ProductId = c.fProductId;
                data.QuantityOfInGameItems ++;
                _context.SaveChanges();
                //dataInCouponTable.MemberId = c.fId;
                //dataInCouponTable.CouponId = cp.fCouponId;
                //dataInCouponTable.CouponStatusId = cp.fCouponStatusId;
            }
            
            //if (data == null)//如果使用者之前沒有優惠券
            //{
            //    var modeldata = new GameItemPurchaseRecord
            //    {
            //        MemberId = c.fId,
            //        ProductId = c.fProductId,
            //        QuantityOfInGameItems = 1,
            //    };
            //    _context.GameItemPurchaseRecord.Add(modeldata);
            //}
            return Ok(new { message = "數據已成功保存" });
        }

    }
}
