using Microsoft.AspNetCore.Mvc;
using prjCatChaOnlineShop.Models;
using prjCatChaOnlineShop.Models.CDictionary;
using prjCatChaOnlineShop.Models.CModels;
using System.Text.Json;

namespace prjCatChaOnlineShop.Controllers.Api
{
    public class CartController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly cachaContext _context;
        public CartController(IHttpContextAccessor httpContextAccessor,cachaContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }


        // 從session裡面刪除購物車商品
        public IActionResult DeleteCartItem(int? id)
        {
            // 檢查傳入的 id 是否有效
            if (id == null)
            {
                return BadRequest("無效的商品 ID");
            }

            // 從 Session 中獲取購物車內容
            string json = "";
            List<CCartItem> cart;
            json = HttpContext.Session.GetString(CDictionary.SK_PURCHASED_PRODUCTS_LIST);
            // 反序列化 Session 中的購物車內容
            cart = JsonSerializer.Deserialize<List<CCartItem>>(json);
            if (cart != null)
            {

                // 找到具有相同 ID 的商品
                var itemToRemove = cart.FirstOrDefault(item => item.cId == id);
                if (itemToRemove != null)
                {
                    // 從購物車刪除找到的商品
                    cart.Remove(itemToRemove);

                    // 更新Session中的購物車內容
                    _httpContextAccessor.HttpContext.Session.SetString(CDictionary.SK_PURCHASED_PRODUCTS_LIST, JsonSerializer.Serialize(cart));

                    decimal total = (decimal)cart.Sum(item => item.c小計);

                    //創建一個匿名對象，包含商品以從購物車刪除的消息以及更新後的總金額
                    var response = new
                    {
                        Message = "商品已從購物車删除",
                        Total = total
                    };
                    return new JsonResult(response);
                }
                else
                {
                    return BadRequest("找不到具有指定 ID 的商品");
                }
            }
            else
            {
                return BadRequest("購物車是空的");
            }
        }
    }
}
