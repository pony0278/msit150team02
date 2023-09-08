using Microsoft.AspNetCore.Mvc;
using prjCatChaOnlineShop.Models.CDictionary;
using prjCatChaOnlineShop.Models.CModels;
using prjCatChaOnlineShop.Models.ViewModels;
using prjCatChaOnlineShop.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using prjCatChaOnlineShop.Areas.AdminCMS.Models.ViewModels;

namespace prjCatChaOnlineShop.Controllers.Api
{
    public class CheckOutController : Controller
    {
        private readonly cachaContext _context;
        public CheckOutController(cachaContext context)
        {
            _context = context;
        }

        //儲存使用者的付款方式、運送方式、取件門市、姓名、電話
        public IActionResult paymentSelected([FromBody] CShippmentModel requestData)
        {
            CShippmentModel shippment = new CShippmentModel
            {
                paymentMethod = requestData.paymentMethod,
                deliveryMethod = requestData.deliveryMethod,
                storeName = requestData.storeName,
                name = requestData.name,
                phone = requestData.phone,
            };
            string json = JsonSerializer.Serialize(shippment);
            HttpContext.Session.SetString(CDictionary.SK_PAYMEMENT_MODEL, json);

            return RedirectToAction("Pay", "Cart");
        }


        public IActionResult StoreCouponId([FromBody] CSelectedCouponId selectedCouponId)
        {
            int Id = selectedCouponId.CouponId;
            HttpContext.Session.SetInt32("CouponId", Id);
            return View();
        }
        public IActionResult AddNewOrder([FromForm] CAddorderViewModel addOrder)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    //會員最後使用的優惠券ID
                    int couponID = HttpContext.Session.GetInt32("CouponId") ?? 0;

                    // 新建訂單
                    ShopOrderTotalTable neworder = new ShopOrderTotalTable
                    {
                        MemberId = addOrder.MemberId,
                        OrderCreationDate = DateTime.Now,
                        OrderStatusId = 2,
                        PaymentMethodId = 2,
                        ShippingMethodId = 2,
                        CouponId = couponID,
                        RecipientAddress = addOrder.RecipientAddress,
                        RecipientName = addOrder.RecipientName,
                        RecipientPhone = addOrder.RecipientPhone,
                        ResultPrice = addOrder.ResultPrice,
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

                    //把該次訂單使用的優惠券CouponStatusID更新為1
                    var updateCouponStatus = _context.ShopMemberCouponData.FirstOrDefault(item => item.MemberId == addOrder.MemberId && item.CouponId == couponID);
                    if (updateCouponStatus != null)
                    {
                        updateCouponStatus.CouponStatusId = true;
                        _context.SaveChanges();
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
