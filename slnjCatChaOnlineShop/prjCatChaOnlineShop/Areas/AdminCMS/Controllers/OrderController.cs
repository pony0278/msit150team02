using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjCatChaOnlineShop.Areas.AdminCMS.Models;
using prjCatChaOnlineShop.Areas.AdminCMS.Models.ViewModels;
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
            //判斷是否有登入
            if (HttpContext.Session.Keys.Contains(CAdminLogin.SK_LOGINED_USER))
            {
                // 讀取管理員姓名
                string adminName = HttpContext.Session.GetString("AdminName");

                // 將管理員姓名傳給view
                ViewBag.AdminName = adminName;

                return View();
            }
            return RedirectToAction("Login", "CMSHome");
        }
        public IActionResult ShowOrderTotal()
        {
            var data = (
                 from order in _context.ShopOrderTotalTable
                 join paymentMethod in _context.ShopPaymentMethodData on order.PaymentMethodId equals paymentMethod.PaymentMethodId
                 join orderStatus in _context.ShopOrderStatusData on order.OrderStatusId equals orderStatus.OrderStatusId
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

            var shippment = _context.ShopShippingMethod
    .FirstOrDefault(sm => sm.ShippingMethodId == orderTotal.ShippingMethodId)?.Shippment;

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
                OrderStatus = orderStatus,
                Shippment = shippment
            };

            return Json(new { data });
        }
        public IActionResult EditOrder(int id)
        {
            var order = _context.ShopOrderTotalTable.FirstOrDefault(o => o.OrderId == id);

            if (order != null)
            {
                //string statusName = _context.ShopOrderStatusData.FirstOrDefault(s => s.OrderStatusId == order.OrderStatusId)?.StatusName; //得到訂單狀態

                return Json(new { data = order });
            }
            else
            {
                // 没有找到匹配的訂單
                // do something..........
                return NotFound();
            }
        }
        [HttpPost]
        public IActionResult UpdateOrder(COrder editOrder)
        {
            var orderData = _context.ShopOrderTotalTable.FirstOrDefault(m => m.OrderId == editOrder.OrderId);
            try
            {
                if (orderData != null) // 檢查 orderData 是否為空
                {
                    orderData.OrderId = editOrder.OrderId;
                    orderData.MemberId = editOrder.MemberId;
                    orderData.RecipientName = editOrder.RecipientName;
                    orderData.RecipientAddress = editOrder.RecipientAddress;
                    orderData.OrderStatusId = editOrder.OrderStatusId;
                    orderData.LastUpdateTime = DateTime.Now;

                    // 存入DbContext
                    _context.SaveChanges();

                    return Json(new { success = true, message = "編輯訂單成功！" });
                }
                else
                {
                    return Json(new { success = false, message = "編輯訂單的資訊為空。" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "編輯會員失敗：" + ex.Message });
            }
        }
    }
}
