using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjCatChaOnlineShop.Models;

namespace prjCatChaOnlineShop.Controllers.CMS
{
    [Area("AdminCMS")]
    public class ServiceController : Controller
    {
        private readonly cachaContext _context;
        public ServiceController(cachaContext context)
        {
            _context = context;
        }
        public IActionResult Service()
        {
            return View();
        }
        public IActionResult ShowComplaintCase()
        {
            var data = (
    from ComplaintCase in _context.ShopMemberComplaintCase

        //撈出客訴狀態
    join ComplaintStatusID in _context.ShopComplaintStatusData on ComplaintCase.ComplaintStatusId equals ComplaintStatusID.ComplaintStatusId

    //撈出申訴類別
    join ComplaintCategoryID in _context.ShopAppealCategoryData on ComplaintCase.ComplaintCategoryId equals ComplaintCategoryID.AppealCategoryId

    //撈出會員姓名
    join memberInfo in _context.ShopMemberInfo on ComplaintCase.MemberId equals memberInfo.MemberId

    select new
    {
        ComplaintCase.ComplaintCaseId,
        ComplaintCase.ComplaintTitle,
        ComplaintCase.CreationTime,
        memberInfo.MemberId,
        memberInfo.Name,
        ComplaintStatusID.ComplaintStatusId,
        ComplaintStatusID.ComplaintStatusName,
        ComplaintCategoryID.AppealCategoryId,
        ComplaintCategoryID.CategoryName,
    }
).ToList();

            return Json(new { data });
        }
        public IActionResult GetCaseDetails(int memberId, int caseId)
        {
            var memberInfo = _context.ShopMemberInfo
            .FirstOrDefault(m => m.MemberId == memberId);

            var complaintCase = _context.ShopComplaintStatusData
                .FirstOrDefault(o => o.ComplaintCaseId == caseId);

            var complaintCaseName = _context.ShopAppealCategoryData
        .FirstOrDefault(c => c.AppealCategoryId == caseId)?.CategoryName;
            //        var orderDetails = _context.ShopOrderDetailTable
            //            .Where(od => od.OrderId == orderId)
            //            .ToList();

            //        var productIds = orderDetails.Select(od => od.ProductId).ToList();

            //        var products = _context.ShopProductTotal
            //            .Where(p => productIds.Contains(p.ProductId))
            //            .ToList();

            //        var paymentMethod = _context.ShopPaymentMethodData
            //            .FirstOrDefault(pm => pm.PaymentMethodId == orderTotal.PaymentMethodId)?.PaymentMethodName;

            //        var shippingMethod = _context.ShopShippingMethod
            //            .FirstOrDefault(sm => sm.ShippingMethodId == orderTotal.ShippingMethodId)?.ShippingMethodName;

            //        var shippment = _context.ShopShippingMethod
            //.FirstOrDefault(sm => sm.ShippingMethodId == orderTotal.ShippingMethodId)?.Shippment;

            //        var couponContent = _context.ShopCouponTotal
            //            .FirstOrDefault(ct => ct.CouponId == orderTotal.CouponId)?.CouponContent;

            //        var orderStatus = _context.ShopOrderStatusData
            //            .FirstOrDefault(os => os.OrderStatusId == orderTotal.OrderStatusId)?.StatusName;

            var data = new
            {
                MemberInfo = memberInfo,
                complaintCase = complaintCase,
                complaintCaseName = complaintCaseName,
                //OrderDetails = orderDetails,
                //Products = products,
                //PaymentMethod = paymentMethod,
                //ShippingMethod = shippingMethod,
                //CouponContent = couponContent,
                //OrderStatus = orderStatus,
                //Shippment = shippment
            };

            return Json(new { data });
        }
    }
}
