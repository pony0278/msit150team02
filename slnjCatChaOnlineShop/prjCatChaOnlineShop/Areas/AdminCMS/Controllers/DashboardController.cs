using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjCatChaOnlineShop.Areas.AdminCMS.Models;
using prjCatChaOnlineShop.Models;
using prjCatChaOnlineShop.Services.Function;

namespace prjCatChaOnlineShop.Areas.AdminCMS.Controllers
{
    [Area("AdminCMS")]
    public class DashboardController : Controller
    {
        private readonly cachaContext _context;
        public DashboardController(cachaContext context)
        {
            _context = context;
        }
        public IActionResult Dashboard()
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
        [HttpGet]
        public async Task<IActionResult> GetApiData()
        {
            try
            {
                // 定義異步任務來獲取數據
                var highPriceMemberTask = GetHighPriceMemberAsync();
                var thisMonthRegistrationsTask = GetThisMonthRegistrationsAsync();
                var ordersCompletedTask = GetOrdersCompletedTaskAsync();
                var ordersCompletedPrice = GetFormattedOrdersCompletedPriceTaskAsync();
                var productTotal = GetProductTaskAsync();
                var productReview = GetProductStarTaskAsync();
                var complaintCase = GetServicePendingTaskAsync();

                // 使用Task.WhenAll等待所有異步任務完成
                await Task.WhenAll(highPriceMemberTask, thisMonthRegistrationsTask, ordersCompletedTask, ordersCompletedPrice, productTotal, productReview, complaintCase);

                // 獲取結果
                var topMember = highPriceMemberTask.Result;
                var thisMonthRegistrations = thisMonthRegistrationsTask.Result;
                var totalOrdersCompleted = ordersCompletedTask.Result;
                var totalOrdersPriceCompleted = ordersCompletedPrice.Result;
                var allProductTotal = productTotal.Result;
                var starAverageScore = productReview.Result;
                var memberComplaintCase = complaintCase.Result;

                // 將返回數據封裝為JSON對象
                var resultData = new
                {
                    TopMember = topMember,
                    ThisMonthRegistrations = thisMonthRegistrations,
                    TotalOrdersCompleted = totalOrdersCompleted,
                    TotalOrdersPriceCompleted = totalOrdersPriceCompleted,
                    AllProductTotal = allProductTotal,
                    StarAverageScore = starAverageScore,
                    MemberComplaintCase = memberComplaintCase
                };

                return Json(new { data = resultData });
            }
            catch (Exception ex)
            {
                // 處理異常
                return BadRequest("發生錯誤：" + ex.Message);
            }
        }

        //控制台出現：A second operation was started on this context instance 錯誤，這是由於在同一實例上進行了DbContext多個並發數據庫操作引起的。 Entity Framework Core默認情況下不允許在同一上下文實例上並行執行多個異步數據庫操作。
        //為了解決這個問題，你可以採取以下方法之一：
        //使用不同的DbContext實例：在每個異步方法中創建一個新的DbContext實例，以確保每個操作都在其自己的上下文中執行。這樣可以避免並發操作衝突。
        private async Task<string> GetHighPriceMemberAsync() //取得單筆訂單金額最高會員與金額
        {
            using (var context = new cachaContext()) //解決同一上下文實例上並行執行多個異步數據庫操作
            {
                DateTime currentDate = DateTime.Now;
                var topOrder = await context.ShopOrderTotalTable
                    .Where(order => order.OrderCreationDate != null &&
                        order.OrderCreationDate.Value.Month == currentDate.Month &&
                        order.OrderCreationDate.Value.Year == currentDate.Year &&
                        order.OrderStatusId != 4) // 確保訂單狀態不是已取消的
                    .OrderByDescending(order => order.ResultPrice) // 按照訂單金額降序排序
                    .FirstOrDefaultAsync();

                if (topOrder != null)
                {
                    var memberName = await context.ShopMemberInfo
                    .Where(member => member.MemberId == topOrder.MemberId)
                    .Select(member => member.Name)
                    .FirstOrDefaultAsync();

                    // 格式化金額，加上千位數逗號，去掉小數點後面的00
                    var formattedAmount = $"{topOrder.ResultPrice:n0}";

                    return $"{memberName} - NT$ {formattedAmount} 元";
                }
                else
                {
                    // 如果本月沒有訂單，返回 null 或其他適當的值
                    return null;
                }
            }
        }

        private async Task<int> GetThisMonthRegistrationsAsync() //取得本月註冊新會員總數量
        {
            using (var context = new cachaContext()) //解決同一上下文實例上並行執行多個異步數據庫操作
            {
                DateTime currentDate = DateTime.Now;
                return await context.ShopMemberInfo
                    .Where(member => member.RegistrationTime != null &&
                        member.RegistrationTime.Value.Month == currentDate.Month &&
                        member.RegistrationTime.Value.Year == currentDate.Year)
                    .CountAsync();
            }
        }

        private async Task<int> GetOrdersCompletedTaskAsync() //取得本月訂單總筆數
        {
            using (var context = new cachaContext()) //解決同一上下文實例上並行執行多個異步數據庫操作
            {
                DateTime currentDate = DateTime.Now;
                return await context.ShopOrderTotalTable.Where(order => order.OrderCreationDate != null &&
                        order.OrderCreationDate.Value.Month == currentDate.Month &&
                        order.OrderCreationDate.Value.Year == currentDate.Year && order.OrderStatusId != 4).CountAsync(); //已取消的訂單不列入計算
            }
        }

        private async Task<string> GetFormattedOrdersCompletedPriceTaskAsync() //取得已成交訂單總金額
        {
            using (var context = new cachaContext()) //解決同一上下文實例上並行執行多個異步數據庫操作
            {
                DateTime currentDate = DateTime.Now;
                decimal totalResultPrice = await context.ShopOrderTotalTable
                .Where(order => order.OrderCreationDate != null &&
                        order.OrderCreationDate.Value.Month == currentDate.Month &&
                        order.OrderCreationDate.Value.Year == currentDate.Year && order.ResultPrice.HasValue && order.OrderStatusId != 4) // 確保ResultPrice有值而且"排除"訂單狀態為已取消的
                .SumAsync(o => o.ResultPrice.Value); // 計算ResultPrice的加總金額

                // 格式化金額，加上千位數逗號
                var formattedAmount = $"NT$ {totalResultPrice:n0} 元";

                return formattedAmount;
            }
        }


        private async Task<string> GetProductTaskAsync() //本月銷售最好產品與售出幾件
        {
            using (var context = new cachaContext())
            {
                DateTime currentDate = DateTime.Now;
                var topSellingProduct = await context.ShopOrderTotalTable
                    .Where(order => order.OrderCreationDate != null &&
                        order.OrderCreationDate.Value.Month == currentDate.Month &&
                        order.OrderCreationDate.Value.Year == currentDate.Year &&
                        order.OrderStatusId != 4) // 確保訂單狀態不是已取消的
                    .Join(context.ShopOrderDetailTable,
                        order => order.OrderId,
                        orderDetail => orderDetail.OrderId,
                        (order, orderDetail) => new
                        {
                            orderDetail.ProductId,
                            orderDetail.ProductQuantity
                        })
                    .GroupBy(orderDetail => orderDetail.ProductId)
                    .Select(group => new
                    {
                        ProductId = group.Key,
                        TotalQuantity = group.Sum(orderDetail => orderDetail.ProductQuantity)
                    })
                    .OrderByDescending(result => result.TotalQuantity)
                    .FirstOrDefaultAsync();

                if (topSellingProduct != null)
                {
                    var productName = await context.ShopProductTotal
                        .Where(product => product.ProductId == topSellingProduct.ProductId)
                        .Select(product => product.ProductName)
                        .FirstOrDefaultAsync();

                    // 返回產品名稱和銷售總數量
                    return $"{productName}－共售出 {topSellingProduct.TotalQuantity} 件";
                }
                else
                {
                    // 如果沒有銷售記錄，返回 null 或其他適當的值
                    return null;
                }
            }
        }

        private async Task<decimal> GetProductStarTaskAsync() //取得產品評價總平均分數
        {
            using (var context = new cachaContext())
            {
                var averageRating = await context.ShopProductReviewTable
        .Where(review => review.ProductRating.HasValue) // 確保有評分值
        .AverageAsync(review => review.ProductRating.Value); // 計算評分的平均值

                // 將平均評分四捨五入到小數點第一位
                var roundedAverageRating = Math.Round(averageRating, 1);

                return roundedAverageRating;
            }
        }

        private async Task<int> GetServicePendingTaskAsync() //取得待處理的客服案件有幾筆
        {
            using (var context = new cachaContext()) //解決同一上下文實例上並行執行多個異步數據庫操作
            {
                return await context.ShopMemberComplaintCase
                .Where(c => c.ComplaintStatusId == 1).CountAsync();
            }
        }
    }
}
