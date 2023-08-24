using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjCatChaOnlineShop.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace prjCatChaOnlineShop.Controllers.CMS
{
    [Area("AdminCMS")]
    public class OrderController : Controller
    {
        private readonly cachaContext _context;
        public OrderController(cachaContext context)
        {
            _context = context;
        }
        public IActionResult Order()
        {
            return View();
        }
        public IActionResult ShowOrderTotal()
        {
            //var data = _context.ShopOrderTotalTable;

            //return Json(new { data });
            var data = (
                 from order in _context.ShopOrderTotalTable
                 join paymentMethod in _context.ShopPaymentMethodData on order.PaymentMethodId equals paymentMethod.PaymentMethodId
                 join orderStatus in _context.ShopOrderStatusData on order.OrderStatusId equals orderStatus.OrderStatusId
                 //join shippingMethod in _context.ShopShippingMethod on order.ShippingMethodId equals shippingMethod.ShippingMethodId
                 join shippingMethod in _context.ShopShippingMethod on order.ShippingMethodId equals shippingMethod.ShippingMethodId into shippingMethods
                 from shippingMethod in shippingMethods.DefaultIfEmpty()
                 join memberInfo in _context.ShopMemberInfo on order.MemberId equals memberInfo.MemberId

                 select new
                 {
                     order.OrderId,
                     memberInfo.MemberId,
                     memberInfo.Name,
                     order.RecipientName,
                     order.RecipientPhone,
                     paymentMethod.PaymentMethodId,
                     paymentMethod.PaymentMethodName,
                     order.OrderCreationDate,
                     orderStatus.OrderStatusId,
                     orderStatus.StatusName,
                     ShippingMethodID = shippingMethod != null ? shippingMethod.ShippingMethodId : (int?)null,
                     ShippingMethodName = shippingMethod != null ? shippingMethod.ShippingMethodName : null,
                     order.LastUpdateTime
                 }
             ).ToList();

            return Json(new { data });
        }

        public IActionResult GetOrderDetails(int memberId, int orderId)
        {
            var memberInfo = _context.ShopMemberInfo
            .FirstOrDefault(m => m.MemberId == memberId);

            var orderTotal = _context.ShopOrderTotalTable
                .FirstOrDefault(o => o.OrderId == orderId);

            var orderDetails = _context.ShopOrderDetailTable
                .Where(od => od.OrderId == orderId)
                .ToList();

            var productIds = orderDetails.Select(od => od.ProductId).ToList();

            var products = _context.ShopProductTotal
                .Where(p => productIds.Contains(p.ProductId))
                .ToList();

            var paymentMethod = _context.ShopPaymentMethodData
                .FirstOrDefault(pm => pm.PaymentMethodId == orderTotal.PaymentMethodId)?.PaymentMethodName;

            var shippingMethod = _context.ShopShippingMethod
                .FirstOrDefault(sm => sm.ShippingMethodId == orderTotal.ShippingMethodId)?.ShippingMethodName;

            var couponContent = _context.ShopCouponTotal
                .FirstOrDefault(ct => ct.CouponId == orderTotal.CouponId)?.CouponContent;

            var orderStatus = _context.ShopOrderStatusData
                .FirstOrDefault(os => os.OrderStatusId == orderTotal.OrderStatusId)?.StatusName;

            var data = new
            {
                MemberInfo = memberInfo,
                OrderTotal = orderTotal,
                OrderDetails = orderDetails,
                Products = products,
                PaymentMethod = paymentMethod,
                ShippingMethod = shippingMethod,
                CouponContent = couponContent,
                OrderStatus = orderStatus
            };

            return Json( new { data });
            //var order = _context.ShopMemberInfo
            //          .Include(m => m.ShopOrderTotalTable)
            //          .Include(m => m.ShopMemberCouponData)
            //          .FirstOrDefault(m => m.MemberId == id);

            //if (member != null)
            //{
            //    foreach (var couponData in member.ShopMemberCouponData)
            //    {
            //        // 根據 couponData.CouponId 查詢其他相關數據並附加到 couponData
            //        var CouponTotal = _context.ShopCouponTotal.FirstOrDefault(o => o.CouponId == couponData.CouponId);
            //    }

            //    return Json(new { data = member });
            //}
            //else
            //{
            //    // 沒有找到匹配的成員
            //    //  do something..........
            //    return NotFound();
            //}
        }
    }
}
