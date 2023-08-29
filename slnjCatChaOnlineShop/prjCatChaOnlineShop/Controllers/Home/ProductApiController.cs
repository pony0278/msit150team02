using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using prjCatChaOnlineShop.Models;
using prjCatChaOnlineShop.Models.CDictionary;
using prjCatChaOnlineShop.Models.CModels;
using prjCatChaOnlineShop.Models.ViewModels;
using prjCatChaOnlineShop.Services.Function;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Xml.Schema;

namespace prjCatChaOnlineShop.Controllers.Home
{
    public class ProductApiController : Controller
    {
        private readonly cachaContext _context;
        private readonly IWebHostEnvironment _host;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ProductService _productService;
        private readonly CheckoutService _checkoutService;
        public ProductApiController(cachaContext context, IWebHostEnvironment host, IHttpContextAccessor httpContextAccessor, ProductService productService, CheckoutService checkoutService)
        {
            _context = context;
            _host = host;
            _httpContextAccessor = httpContextAccessor;
            _productService = productService;
            _checkoutService = checkoutService;

        }
        public IActionResult Index()
        {
            return View();
        }
        //修改數量
        [HttpPost]
        public IActionResult CartEditQuantity(int newQuantity, int pId, string attr) 
        {
            string json = "";
            List<CCartItem> cart;
            if (attr != null)
            { 
                json = HttpContext.Session.GetString(CDictionary.SK_PURCHASED_PRODUCTS_LIST);
                cart = JsonSerializer.Deserialize<List<CCartItem>>(json);
                var existingCartItem = cart.FirstOrDefault(item => item.cId == pId&&item.c子項目== attr);
                existingCartItem.c數量 = newQuantity;
                // 將更新後的購物車列表序列化成 JSON，並存入 Session 變數中
                SaveCart(cart);
            }
            else
            {
                json = HttpContext.Session.GetString(CDictionary.SK_PURCHASED_PRODUCTS_LIST);
                cart = JsonSerializer.Deserialize<List<CCartItem>>(json);
                var existingCartItem = cart.FirstOrDefault(item => item.cId == pId);
                existingCartItem.c數量 = newQuantity;
                // 將更新後的購物車列表序列化成 JSON，並存入 Session 變數中
                SaveCart(cart);
            }
            return Json(new { success = true });
        }
        //修改子項目
        [HttpPost]
        public IActionResult CartEditAttribute(string oldAttribute, string newAttribute, int pId)
        {
            string json = "";
            List<CCartItem> cart;
            json = HttpContext.Session.GetString(CDictionary.SK_PURCHASED_PRODUCTS_LIST);
            cart = JsonSerializer.Deserialize<List<CCartItem>>(json);
            //找到跟要調整的子選項一模一樣的商品
            var existingCartItem = cart.FirstOrDefault(item => item.cId == pId && item.c子項目 == newAttribute);
            //找到正在調整的的商品
            var oldCartItem = cart.FirstOrDefault(item => item.cId == pId && item.c子項目 == oldAttribute);
            if (existingCartItem != null)
            {
                //先將相同的品項數量相加
                int? totalQuantity = existingCartItem.c數量+oldCartItem.c數量;
                // 移除所有重複項目，加入一個新的
                cart.Remove(existingCartItem);
                cart.Remove(oldCartItem);
                var prodItem = _productService.getProductById(pId);
                _productService.detailsAddCartItem(cart, prodItem, newAttribute, totalQuantity);
                

                // 將更新後的購物車列表序列化成 JSON，並存入 Session 變數中
                SaveCart(cart);
            }
            else
            {
                //先將新的加入
                var prodItem = _productService.getProductById(pId);
                _productService.detailsAddCartItem(cart, prodItem, newAttribute, oldCartItem.c數量);
                //再刪除舊的
                cart.Remove(oldCartItem);
                
                SaveCart(cart);
            }
            // 將更新後的購物車列表序列化成 JSON，並存入 Session 變數中
            return Json(new { success = true});
        }


        //點擊加入購物車按鈕加入購物車
        [HttpPost]
        public IActionResult AddToCart(int pId)
        {
            if (GetCurrentMemberId() != null)
            {
                var prodItem = _productService.getProductById(pId);

                var cart = GetCartFromSession();


                // 調用簡化方法，傳入產品物件和數量
                _productService.addCartItem(cart, prodItem, 1);
                SaveCart(cart);
                return Json(new { success = true, message = "已加入購物車!" });
            }
            return Json(new { success = false, message="請先登入!" });
        }
        //在details點擊加入購物車按鈕加入購物車
        [HttpPost]
        public IActionResult DetailsAddToCart(int pId, string attr, int count)
        {

            if (GetCurrentMemberId() != null)
            {
                var prodItem = _productService.getProductById(pId);
                // 接下來進行購物車處理...
                var cart = GetCartFromSession();


                // 調用簡化方法，傳入產品物件和數量
                _productService.detailsAddCartItem(cart, prodItem, attr, count);
                SaveCart(cart);


                return Json(new { success = true, message = "已加入購物車!" });
            }
            return Json(new { success = false, message = "請先登入!" });

        }

        //找到存取購物車的session
        private List<CCartItem> GetCartFromSession()
        {
            string json = "";
            if (HttpContext.Session.Keys.Contains(CDictionary.SK_PURCHASED_PRODUCTS_LIST))
            {
                json = HttpContext.Session.GetString(CDictionary.SK_PURCHASED_PRODUCTS_LIST);
                return JsonSerializer.Deserialize<List<CCartItem>>(json);
            }
            else
            {
                return new List<CCartItem>();
            }
        }
        //存進session
        private void SaveCart(List<CCartItem> cart)
        {
            string json = JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString(CDictionary.SK_PURCHASED_PRODUCTS_LIST, json);
        }
        public IActionResult ShopItemPerPage(string? catName, int itemPerPage)
        {

            if (catName != null)//有選category才會傳入catName
            {
                List<CCategoryItem> categoryItems = new List<CCategoryItem>();
                var items = _productService.getProductByCategoryName(catName).Take(itemPerPage);
                var name = items.FirstOrDefault()?.pCategoryName;
                CCategoryItem c = new CCategoryItem();
                c.pItem = items.ToList();
                c.categoryName = name;
                categoryItems.Add(c);
                return Json(categoryItems);
            }
            else
            {
                var allItems = _productService.getProductItems().Take(itemPerPage);//只出現商品名稱不同的品項
                return Json(allItems);
            }

        }

        public IActionResult GetDetails(int? pId)
        {
            var prodItem = _productService.getProductById(pId);

            var cart = GetCartFromSession();


            // 調用簡化方法，傳入產品物件和數量
            _productService.addCartItem(cart, prodItem, 1);
            SaveCart(cart);
            return RedirectToAction("Shop");
        }
        //找到會員ID
        private int? GetCurrentMemberId()
        {
            var memberInfoJson = _httpContextAccessor.HttpContext?.Session.GetString(CDictionary.SK_LOINGED_USER);
            if (memberInfoJson != null)
            {
                var memberInfo = JsonSerializer.Deserialize<ShopMemberInfo>(memberInfoJson);
                return memberInfo.MemberId;
            }

            return null;
        }

        public IActionResult AddToWishlist(int? pId)
        {
            if(GetCurrentMemberId() != null)
            {
                var existingItem = (from w in _context.ShopFavoriteDataTable.AsEnumerable()
                                where w.MemberId== GetCurrentMemberId() && w.ProductId==pId
                                select w).FirstOrDefault();

                //判斷是否已加入
                if (existingItem!=null)
                {
                    _context.ShopFavoriteDataTable.Remove(existingItem);
                }
                else
                {
                    // 如果清單中沒有該 pId 的商品，則加入該商品到資料表
                    var newItem = new ShopFavoriteDataTable
                    {
                    
                        MemberId = GetCurrentMemberId(),
                        ProductId = pId,
                        CreationDate = DateTime.Now
                    };
                    _context.ShopFavoriteDataTable.Add(newItem);
                }
                _context.SaveChanges();
                return Json(new { success = true });

            }
            return Json(new { success = false, message = "請先登入!" });
        }

    }
}
