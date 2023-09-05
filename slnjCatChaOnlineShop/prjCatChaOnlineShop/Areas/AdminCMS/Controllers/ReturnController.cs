using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using prjCatChaOnlineShop.Areas.AdminCMS.Models;
using prjCatChaOnlineShop.Models;
using System.Drawing;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace prjCatChaOnlineShop.Controllers.CMS
{
    [Area("AdminCMS")]
    public class ReturnController : Controller
    {
        private readonly cachaContext _context;
        public ReturnController(cachaContext context)
        {
            _context = context;
        }
        public IActionResult Return()
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
        public IActionResult ShowReturnTotal()
        {
            var data = (
    from orderReturn in _context.ShopReturnDataTable
    join ReturnReasonID in _context.ShopReturnReasonDataTable on orderReturn.ReturnReasonId equals ReturnReasonID.ReturnReasonId
    join ProcessingStatusID in _context.ShopReturnStatusDataTable on orderReturn.ProcessingStatusId equals ProcessingStatusID.ProcessingStatusId
    join orderTotal in _context.ShopOrderTotalTable on orderReturn.OrderId equals orderTotal.OrderId
    join memberInfo in _context.ShopMemberInfo on orderTotal.MemberId equals memberInfo.MemberId
    select new
    {
        orderReturn.OrderId,
        orderTotal.MemberId,
        memberInfo.Name,
        orderReturn.ReturnDate,
        ReturnReasonID.ReturnReason,
        ProcessingStatusID.StatusName
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

            var returnData = _context.ShopReturnDataTable.
                FirstOrDefault(r => r.OrderId == orderId);

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

            var returnProduct = _context.ShopProductTotal
                .FirstOrDefault(rp => rp.ProductId == returnData.ProductId)?.ProductName;

            var returnReason = _context.ShopReturnReasonDataTable
               .FirstOrDefault(rr => rr.ReturnReasonId == returnData.ReturnReasonId)?.ReturnReason;

            var returnStatus = _context.ShopReturnStatusDataTable
               .FirstOrDefault(rs => rs.ProcessingStatusId == returnData.ProcessingStatusId)?.StatusName;

            var data = new
            {
                MemberInfo = memberInfo,
                OrderTotal = orderTotal,
                ReturnData = returnData,
                OrderDetails = orderDetails,
                Products = products,
                PaymentMethod = paymentMethod,
                ShippingMethod = shippingMethod,
                CouponContent = couponContent,
                OrderStatus = orderStatus,
                Shippment = shippment,
                ReturnProduct = returnProduct,
                ReturnReason = returnReason,
                ReturnStatus = returnStatus
            };

            return Json(new { data });
        }
        public IActionResult EditReturn(int id)
        {
            var orderReturn = _context.ShopReturnDataTable.FirstOrDefault(o => o.OrderId == id);

            if (orderReturn != null)
            {
                //得到資料庫裡所有原因名稱並放到select option裡面
                var returnReasonSelect = _context.ShopReturnReasonDataTable
                            .Select(c => c.ReturnReason)
                            .ToList();

                string returnReasonNames = _context.ShopReturnReasonDataTable.FirstOrDefault(s => s.ReturnReasonId == orderReturn.ReturnReasonId)?.ReturnReason; //得到原因名稱

                //得到資料庫裡所有狀態名稱並放到select option裡面
                var returnStatusSelect = _context.ShopReturnStatusDataTable
                            .Select(c => c.StatusName)
                            .ToList();

                string returnStatusNames = _context.ShopReturnStatusDataTable.FirstOrDefault(s => s.ProcessingStatusId == orderReturn.ProcessingStatusId)?.StatusName; //得到原因名稱

                var data = new
                {
                    OrderReturn = orderReturn,
                    ReturnReasonSelect = returnReasonSelect,
                    ReturnReasonNames = returnReasonNames,
                    ReturnStatusSelect = returnStatusSelect,
                    ReturnStatusNames = returnStatusNames,
                };

                return Json(new { data });
            }
            else
            {
                // 没有找到匹配的訂單
                // do something..........
                return NotFound();
            }
        }
        [HttpPost]
        public IActionResult UpdateReturn(CReturn editReturn)
        {
            var returnData = _context.ShopReturnDataTable.FirstOrDefault(m => m.OrderId == editReturn.OrderId);
            try
            {
                if (returnData != null) // 檢查 returnData 是否為空
                {
                    returnData.OrderId = editReturn.OrderId;
                    returnData.ProcessingStatusId = editReturn.ProcessingStatusId;
                    returnData.ReturnReasonId = editReturn.ReturnReasonId;

                    // 存入DbContext
                    _context.Entry(returnData).State = EntityState.Modified;
                    _context.SaveChanges();

                    //_context.SaveChanges();
                    return Json(new { success = true, message = "編輯成功！" });
                }
                else
                {
                    return Json(new { success = false, message = "編輯的資訊為空。" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "編輯失敗：" + ex.Message });
            }
        }
        public IActionResult ReasonList()
        {
            var serviceCase = _context.ShopReturnReasonDataTable;

            if (serviceCase != null)
            {

                //得到資料庫裡所有分類名稱並放到select option裡面
                var complaintCategorySelect = _context.ShopReturnReasonDataTable
                            .Select(c => c.ReturnReason)
                            .ToList();

                var data = new
                {
                    ServiceCase = serviceCase,
                    ComplaintCategorySelect = complaintCategorySelect,
                };

                return Json(new { data });
            }
            else
            {
                return NotFound();
            }
        }
        [HttpPost]
        public IActionResult AddReturnReason(List<ShopReturnReasonDataTable> reasons)
        {
            try
            {
                foreach (var reason in reasons)
                {
                    _context.ShopReturnReasonDataTable.Add(reason);
                    _context.SaveChanges();
                }

                return Json(new { success = true, message = "編輯成功！" });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return Json(new { success = false, message = "編輯失敗：" + ex.Message });
            }
        }
    }
}
