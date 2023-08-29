using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Experimental.ProjectCache;
using prjCatChaOnlineShop.Models;
using prjCatChaOnlineShop.Models.CDictionary;
using prjCatChaOnlineShop.Models.CModels;
using prjCatChaOnlineShop.Models.ViewModels;
using prjCatChaOnlineShop.Services.Function;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Web;

namespace prjCatChaOnlineShop.Controllers.Home
{
    public class CartController : Controller
    {
        private readonly cachaContext _context;
        private readonly CheckoutService _checkoutService;
        private readonly IWebHostEnvironment _host;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ProductService _productService;
        
        public CartController(cachaContext context,CheckoutService checkoutService ,IWebHostEnvironment host, IHttpContextAccessor httpContextAccessor, ProductService productService)
        {
            _context = context;
            _host = host;
            _httpContextAccessor = httpContextAccessor;
            _productService = productService;
            _checkoutService = checkoutService;
            
        }
        public IActionResult ConfrimOrder()
        {
            string userName = HttpContext.Session.GetString("UserName");
            ViewBag.UserName = userName;//把使用者名字傳給_Layout
            return View();
        }
        public IActionResult Checkout() 
        {
            string userName = HttpContext.Session.GetString("UserName");
            ViewBag.UserName = userName;//把使用者名字傳給_Layout

            string loginUser = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER);
            string productList = HttpContext.Session.GetString(CDictionary.SK_PURCHASED_PRODUCTS_LIST);

            if (loginUser != null && productList != null)
            {
                // 從 Session 中讀取抓到的 MEMBER ID
                var memberInfoJson = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER);
                var memberInfo = JsonSerializer.Deserialize<ShopMemberInfo>(memberInfoJson);

                // 使用 CheckoutService 來獲取可用的優惠券
                var usableCoupons = _checkoutService.GetUsableCoupons(memberInfo.MemberId);
                // 使用 CheckoutService 來獲取儲存的地址
                var usableAddress = _checkoutService.GetUsableAddresses(memberInfo.MemberId);
                //購物車
                var cartItems = JsonSerializer.Deserialize<List<CCartItem>>(productList);

                //創建綠界訂單
                var orderId = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 20);
                //需填入你的網址
                var website = $"https://localhost:7218";
                var order = new Dictionary<string, string>
          {
            //綠界需要的參數
            { "MerchantTradeNo",  orderId},
            { "MerchantTradeDate",  DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")},
            { "TotalAmount",  "100"},
            { "TradeDesc",  "無"},
            { "ItemName",  "測試商品"},
            { "ExpireDate",  "3"},
            { "CustomField1",  ""},
            { "CustomField2",  ""},
            { "CustomField3",  ""},
            { "CustomField4",  ""},
            { "ReturnURL",  $"{website}/Ecpay/AddPayInfo"},
            { "OrderResultURL", $"{website}/EcpayHome/PayInfo/{orderId}"},
            { "PaymentInfoURL",  $"{website}/Ecpay/AddAccountInfo"},
            { "ClientRedirectURL",  $"{website}/EcpayHome/AccountInfo/{orderId}"},
            { "MerchantID",  "2000132"},
            { "IgnorePayment",  "GooglePay#WebATM#CVS#BARCODE"},
            { "PaymentType",  "aio"},
            { "ChoosePayment",  "ALL"},
            { "EncryptType",  "1"},
          };
                //檢查碼
                order["CheckMacValue"] = GetCheckMacValue(order);

                var viewModel = new CCheckoutViewModel
                {
                    memberUsableCoupon = usableCoupons ?? new List<CGetUsableCouponModel>(), // 初始化為空列表
                    memberUsableAddress = usableAddress ?? new List<CgetUsableAddressModel>(),
                    cartItems = cartItems ?? new List<CCartItem>(),
                    keyValuePairs = order ?? new Dictionary<string, string>()
                };

                
                return View(viewModel);
            }
            return View();
        }
        private string GetCheckMacValue(Dictionary<string, string> order)
        {
            var param = order.Keys.OrderBy(x => x).Select(key => key + "=" + order[key]).ToList();
            var checkValue = string.Join("&", param);
            //測試用的 HashKey
            var hashKey = "5294y06JbISpM5x9";
            //測試用的 HashIV
            var HashIV = "v77hoKGq4kWxNNIS";
            checkValue = $"HashKey={hashKey}" + "&" + checkValue + $"&HashIV={HashIV}";
            checkValue = HttpUtility.UrlEncode(checkValue).ToLower();
            checkValue = GetSHA256(checkValue);
            return checkValue.ToUpper();
        }
        private string GetSHA256(string value)
        {
            // EncryptType CheckMacValue加密類型
            var result = new StringBuilder();
            var sha256 = SHA256.Create();
            var bts = Encoding.UTF8.GetBytes(value);
            var hash = sha256.ComputeHash(bts);
            for (int i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString("X2"));
            }
            return result.ToString();
        }
        public IActionResult Cart()
        {
            string userName = HttpContext.Session.GetString("UserName");
            ViewBag.UserName = userName;//把使用者名字傳給_Layout

            string json = HttpContext.Session.GetString(CDictionary.SK_PURCHASED_PRODUCTS_LIST);
            if (json == null)
            {

                return View();
            }
            else
            {            
                List<CCartItem> cart = JsonSerializer.Deserialize<List<CCartItem>>(json);
                return View(cart);
            }
        }

        // 刪除購物車商品
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
                var itemToRemove = cart.FirstOrDefault(item=> item.cId==id);
                if (itemToRemove != null)
                {
                    // 从购物车中删除找到的商品
                    cart.Remove(itemToRemove);

                    // 更新 Session 中的购物车内容
                    _httpContextAccessor.HttpContext.Session.SetString(CDictionary.SK_PURCHASED_PRODUCTS_LIST, JsonSerializer.Serialize(cart));

                    return Ok("商品已從購物車刪除");
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


        ////刪除購物車商品
        //public IActionResult DeleteCartItem(int? id)
        //{
        //    var memberCartInfoJson = _httpContextAccessor.HttpContext?.Session.GetString(CDictionary.SK_PURCHASED_PRODUCTS_LIST);
        //    if (memberCartInfoJson != null)
        //    {
        //        var memberCartInfo = JsonSerializer.Deserialize<CCartItem>(memberCartInfoJson);
        //        var trashItem= memberCartInfo.Items

        //        _httpContextAccessor.HttpContext.Session.SetString(CDictionary.SK_PURCHASED_PRODUCTS_LIST, JsonSerializer.Serialize(memberCartInfo));
        //    }
        //}
    }
}
