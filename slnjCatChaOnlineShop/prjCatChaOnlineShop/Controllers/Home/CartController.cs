using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Experimental.ProjectCache;
using Microsoft.EntityFrameworkCore.Infrastructure;
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

        #region 注入服務
        public CartController(cachaContext context, CheckoutService checkoutService, IWebHostEnvironment host, IHttpContextAccessor httpContextAccessor, ProductService productService)
        {
            _context = context;
            _host = host;
            _httpContextAccessor = httpContextAccessor;
            _productService = productService;
            _checkoutService = checkoutService;

        }
        #endregion

        #region 購物車頁面
        public IActionResult Cart()
        {
            string userName = HttpContext.Session.GetString("UserName");
            ViewBag.UserName = userName;//把使用者名字傳給_Layout
            ViewBag.Categories = _productService.getAllCategories();//把類別傳給_Layout

            string json = HttpContext.Session.GetString(CDictionary.SK_PURCHASED_PRODUCTS_LIST);
            if (json == null)
            {
                return View();
            }
            else
            {
                List<CCartItem> cart = JsonSerializer.Deserialize<List<CCartItem>>(json);
                decimal total = (decimal)cart.Sum(item => item.c小計);
                ViewBag.totalPrice = total; //把初始的小計金額傳到cart頁面
                return View(cart);
            }
        }
        #endregion

        #region 填寫結帳資料頁面
        public IActionResult Checkout()
        {
            string userName = HttpContext.Session.GetString("UserName");
            ViewBag.UserName = userName;//把使用者名字傳給_Layout
            ViewBag.Categories = _productService.getAllCategories();//把類別傳給_Layout

            string loginUser = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER);
            string productList = HttpContext.Session.GetString(CDictionary.SK_PURCHASED_PRODUCTS_LIST);

            if (loginUser != null && productList != null)
            {
                // 從 Session 中讀取抓到的 MEMBER ID
                var memberInfoJson = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER);
                var memberInfo = JsonSerializer.Deserialize<ShopMemberInfo>(memberInfoJson);

                // 使用CheckoutService來獲取可用的優惠券
                var usableCoupons = _checkoutService.GetUsableCoupons(memberInfo.MemberId);
                // 使用CheckoutService來獲取儲存的地址
                var usableAddress = _checkoutService.GetUsableAddresses(memberInfo.MemberId);
                // 使用CheckoutService來取得可折抵的金額及紅利
                var usableBonus = _checkoutService.GetLoyaltyPoints();

                //購物車
                var cartItems = JsonSerializer.Deserialize<List<CCartItem>>(productList);

                //結帳頁面-購物車初始小計金額
                decimal total = (decimal)cartItems.Sum(item => item.c小計);
                ViewBag.totalPrice = total; //把初始的小計金額傳到checkout頁面

                //結帳頁面-購物車初始運費
                decimal firstFee = 0;
                if (total < 2000)
                {
                    firstFee = 60;
                }
                ViewBag.firstFee = firstFee; //把初始的初始運費傳到checkout頁面

                //結帳頁面-購物車初始總計
                decimal firstTotalPrice = total + firstFee;
                ViewBag.firstTotalPrice = firstTotalPrice;

                //把初始總計先存在session裡面，綠界參數會需要
                HttpContext.Session.SetString(CDictionary.SK_FINALTOTALPRICE, Convert.ToString(firstTotalPrice));

                CPayModel paymodel = new CPayModel
                {
                    subtotal = (int?)total,
                    shippingFee=firstFee,
                    finalBonus=0,
                    finalAmount= (int?)firstTotalPrice,
                };
                //轉換成JSON
                string json = JsonSerializer.Serialize(paymodel);
                //存在session
                HttpContext.Session.SetString(CDictionary.SK_PAY_MODEL, json);

                var viewModel = new CCheckoutViewModel
                {
                    memberUsableCoupon = usableCoupons ?? new List<CGetUsableCouponModel>(),
                    memberUsableAddress = usableAddress ?? new List<CgetUsableAddressModel>(),
                    cartItems = cartItems ?? new List<CCartItem>(),
                    getCouponPrice = usableBonus ?? new CGetCouponPrice(),
                };


                return View(viewModel);
            }
            return View();
        }
        #endregion

        #region 付款頁面

        public IActionResult Pay()
        {
            string userName = HttpContext.Session.GetString("UserName");
            ViewBag.UserName = userName;//把使用者名字傳給_Layout
            ViewBag.Categories = _productService.getAllCategories();//把類別傳給_Layout

            //從session中拿取會員資料
            string memberInfo = HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER);
            //從session中拿取購物車的資料
            string productList = HttpContext.Session.GetString(CDictionary.SK_PURCHASED_PRODUCTS_LIST);
            //從session中拿取最後計算好的金額資料
            string priceData = HttpContext.Session.GetString(CDictionary.SK_PAY_MODEL);
            //從session中拿取最後選擇的付款方式跟運送方式
            string paymentMethod = HttpContext.Session.GetString(CDictionary.SK_PAYMEMENT_MODEL);

            if (memberInfo != null && productList != null && priceData != null)
            {
                //會員資料
                var member = JsonSerializer.Deserialize<ShopMemberInfo>(memberInfo);
                var memberid = member.MemberId;
                ViewBag.MemberId = memberid;

                //購物車資料
                var cartItems = JsonSerializer.Deserialize<List<CCartItem>>(productList);

                //建構商品名稱的字串，綠界用#字號來換行
                string cartItemName = string.Join("#", cartItems.Select(item => item.cName));

                //最後金額資料
                var finalpriceData = JsonSerializer.Deserialize<CPayModel>(priceData);
                //
                // 獲取 finalTotalPrice 的值
                var finalTotalPrice = HttpContext.Session.GetString(CDictionary.SK_FINALTOTALPRICE);

                //獲取最後運送方式、付款方式、姓名、電話等資料
                var finalpaymentMethod = JsonSerializer.Deserialize<CShippmentModel>(paymentMethod);

                //創建綠界訂單
                var orderId = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 20);
                //需填入你的網址
                var website = $"https://localhost:7218";
                var order = new Dictionary<string, string>
          {

            //綠界需要的參數
            { "MerchantTradeNo",  orderId},
            { "MerchantTradeDate",  DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")},
            { "TotalAmount",finalTotalPrice },
            { "TradeDesc",  "無"},
            { "ItemName",  cartItemName},
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

                var viewmodel = new CCheckoutViewModel
                {
                    keyValuePairs = order ?? new Dictionary<string, string>(),
                    cartItems = cartItems ?? new List<CCartItem>(),
                    getFinalPriceData = finalpriceData ?? new CPayModel(),
                    getFinalShippmentData = finalpaymentMethod ?? new CShippmentModel(),
                };
                return View(viewmodel);
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
        #endregion

        #region 確認訂單
        public IActionResult ConfrimOrder()
        {
            //結完帳就把購物車的session清空
            HttpContext.Session.Remove(CDictionary.SK_PURCHASED_PRODUCTS_LIST);

            string userName = HttpContext.Session.GetString("UserName");
            ViewBag.UserName = userName;//把使用者名字傳給_Layout
            ViewBag.Categories = _productService.getAllCategories();//把類別傳給_Layout

            return View();
        }
        #endregion





    }
}
