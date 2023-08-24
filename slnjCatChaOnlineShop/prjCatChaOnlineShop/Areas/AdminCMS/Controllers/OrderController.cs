using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjCatChaOnlineShop.Models;

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
    }
}
