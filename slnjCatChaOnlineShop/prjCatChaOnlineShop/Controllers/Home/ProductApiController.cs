using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json.Linq;
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

        #region 加入收藏
        public IActionResult AddToWishlist(int? pId)
        {
            if (_productService.GetCurrentMemberId() != null)
            {
                var existingItem = (from w in _context.ShopFavoriteDataTable.AsEnumerable()
                                    where w.MemberId == _productService.GetCurrentMemberId() && w.ProductId == pId
                                    select w).FirstOrDefault();                
                //判斷是否已加入
                if (existingItem != null)//移除
                {
                    _context.ShopFavoriteDataTable.Remove(existingItem);
                    _context.SaveChanges();
                    return Json(new { success = true,message = "cancel" });
                }
                else
                {
                    // 如果清單中沒有該 pId 的商品，則加入該商品到資料表
                    var newItem = new ShopFavoriteDataTable
                    {

                        MemberId = _productService.GetCurrentMemberId(),
                        ProductId = pId,
                        CreationDate = DateTime.Now
                    };
                    _context.ShopFavoriteDataTable.Add(newItem);
                    _context.SaveChanges();
                    return Json(new { success = true, message="favorited" });
                }
                
            }
            //未登入的情況
            return Json(new { success = false, message = "請先登入!" });
        }
        #endregion

        #region 加入購物車
        //修改數量
        [HttpPost]
        public IActionResult CartEditQuantity(int newQuantity, int pId, string attr) 
        {
            string json = "";
            List<CCartItem> cart;
            
            json = HttpContext.Session.GetString(CDictionary.SK_PURCHASED_PRODUCTS_LIST);
            cart = JsonSerializer.Deserialize<List<CCartItem>>(json);
            var existingCartItem = cart.FirstOrDefault(item => item.cId == pId && item.c子項目 == attr);
            if (existingCartItem == null)
            {
                existingCartItem = cart.FirstOrDefault(item => item.cId == pId);
                existingCartItem.c數量 = newQuantity;
            }
            else
            {
                existingCartItem.c數量 = newQuantity;
                // 將更新後的購物車列表序列化成 JSON，並存入 Session 變數中
            }
            //購物車商品小計
            decimal totalPrice = 0;
            totalPrice = (decimal)cart.Sum(item => item.c小計);
            SaveCart(cart);
            return Json(new { success = true,message= existingCartItem.c小計, messageTotal = totalPrice });
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
            if (_productService.GetCurrentMemberId() != null)
            {
                var prodItem = _productService.getProductById(pId);

                var cart = GetCartFromSession();
                // 調用簡化方法，傳入產品物件和數量
                try
                {
                    _productService.addCartItem(cart, prodItem, 1);
                    SaveCart(cart);

                    return Json(new { success = true, message = "已加入購物車!" });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, messageNoQuantity = ex.Message }); // 返回庫存不足的訊息
                }
            }

            return Json(new { success = false, message="請先登入!" });
        }
        //在details點擊加入購物車按鈕加入購物車
        [HttpPost]
        public IActionResult DetailsAddToCart(int pId, string attr, int count)
        {

            if (_productService.GetCurrentMemberId() != null)
            {
                var prodItem = _productService.getProductById(pId);
                // 接下來進行購物車處理...
                var cart = GetCartFromSession();
                // 調用簡化方法，傳入產品物件和數量
                try
                {
                    _productService.detailsAddCartItem(cart, prodItem, attr, count);
                    SaveCart(cart);

                    return Json(new { success = true, message = "已加入購物車!" });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, messageNoQuantity = ex.Message }); // 返回庫存不足的訊息
                }
            }
            return Json(new { success = false, message = "請先登入!" });

        }
        //得到購物車商品數量(非總商品數)
        public IActionResult GetCartItemsCount()
        {
            int cartCount = GetCartFromSession().Count();
            return Json(cartCount);
        }

        //找到存取購物車的session
        private List<CCartItem> GetCartFromSession()
        {
            if (HttpContext.Session.Keys.Contains(CDictionary.SK_PURCHASED_PRODUCTS_LIST))
            {
                string json = HttpContext.Session.GetString(CDictionary.SK_PURCHASED_PRODUCTS_LIST);
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

        #endregion

        #region 多重篩選
        public IActionResult MultipleFilter(int optionOrder, string? optionBrand, string? catName, int itemPerPage)
        {
            var prods = _productService.getProductItems();
            if (catName != null)
            {
                prods= _productService.getProductByCategoryName(catName);
            }
            if (optionBrand != null)
            {
                prods = prods.Where(p => p.pName.Contains(optionBrand)).ToList();
            }
            switch (optionOrder)
            {
                // 商品排序
                case 0:
                break;
                //熱賣程度:高到低
                case 1:
                    prods = prods.OrderBy(item => item.p剩餘庫存).ToList();
                    break;
                //熱賣程度:低到高
                case 2:
                    prods = prods.OrderByDescending(item => item.p剩餘庫存).ToList();
                    break;
                //上架時間:新到舊
                case 3:
                    prods = prods.OrderByDescending(item => item.p上架時間).ToList();
                    break;
                //上架時間:舊到新
                case 4:
                    prods = prods.OrderBy(item => item.p上架時間).ToList();
                    break;
                //價格:高至低
                case 5:
                    prods = prods.OrderByDescending(item => _productService.priceFinal(item.pPrice, item.p優惠價格)).ToList();
                    break;
                //價格:低至高
                case 6:
                    prods = prods.OrderBy(item => _productService.priceFinal(item.pPrice, item.p優惠價格)).ToList();
                    break;
            }
            
            return Json(prods.Take(itemPerPage));
            
        }
        #endregion

        //置頂公告
        public IActionResult GetTopAnnouncement()
        {
            var data = _context.GameShopAnnouncement
                .Where(a => a.PinToTop == true &&a.DisplayOrNot != true)
                .OrderBy(a=>a.AnnouncementTypeId)
                .Select(a=>a.AnnouncementTitle)
                .FirstOrDefault();

            if (data != null)
            {
                return Json(new { success = true, message = data });

            }
            return Json(new { success = false, messageWelcome = " 歡迎光臨catCha貓抓抓商城" });
        }
    }
}
