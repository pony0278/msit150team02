using Microsoft.AspNetCore.Mvc;
using prjCatChaOnlineShop.Models.CDictionary;
using prjCatChaOnlineShop.Models.CModels;
using prjCatChaOnlineShop.Models.ViewModels;
using prjCatChaOnlineShop.Models;
using System.Text.Json;

namespace prjCatChaOnlineShop.Controllers.Api
{
    public class CheckOutController : Controller
    {
        private readonly cachaContext _context;
        public CheckOutController(cachaContext context)
        { 
        _context = context;
        }
        public IActionResult AddNewOrder([FromForm] CAddorderViewModel addOrder)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // 新建訂單
                    ShopOrderTotalTable neworder = new ShopOrderTotalTable
                    {
                        MemberId = addOrder.MemberId,
                        OrderCreationDate = DateTime.Now,
                        OrderStatusId = 2,
                        PaymentMethodId = 2,
                        CouponId = addOrder.CouponId
                    };
                    _context.Add(neworder);
                    _context.SaveChanges(); // 

                    // 獲取新建訂單的 Order ID
                    int newOrderId = neworder.OrderId;

                    // 獲取存儲在 Session 中的購買商品資訊
                    string productList = HttpContext.Session.GetString(CDictionary.SK_PURCHASED_PRODUCTS_LIST);

                    // 反序列購物車的商品資訊
                    var cartItems = JsonSerializer.Deserialize<List<CCartItem>>(productList);

                    // 將訂單詳細資料存入 ShopOrderDetailTable
                    foreach (var product in cartItems)
                    {
                        ShopOrderDetailTable orderDetail = new ShopOrderDetailTable
                        {
                            OrderId = newOrderId,
                            ProductId = product.cId,
                            ProductQuantity = product.c數量,
                        };
                        _context.Add(orderDetail);

                        //更新商品庫存
                        var productToUpdate = _context.ShopProductTotal.FirstOrDefault(item => item.ProductId == product.cId);
                        if (productToUpdate != null)
                        {
                            productToUpdate.RemainingQuantity = productToUpdate.RemainingQuantity - product.c數量;
                            _context.SaveChanges();
                        }
                    }
                    transaction.Commit(); // 提交事務
                    return View();
                }
                catch (Exception ex)
                {
                    // 處理異常，例如回滾事務
                    transaction.Rollback();
                    // 返回錯誤頁面或錯誤訊息
                    return View("Error");
                }
            }




        }
    }
}
