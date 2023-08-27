using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using prjCatChaOnlineShop.Models;
using prjCatChaOnlineShop.Models.CDictionary;
using prjCatChaOnlineShop.Models.CModels;
using prjCatChaOnlineShop.Models.ViewModels;
using prjCatChaOnlineShop.Services.Function;
using System.Collections.Generic;
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
        public ProductApiController(cachaContext context, IWebHostEnvironment host, IHttpContextAccessor httpContextAccessor, ProductService productService)
        {
            _context = context;
            _host = host;
            _httpContextAccessor = httpContextAccessor;
            _productService = productService;

        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ShopItemPerPage(string? catName, int itemPerPage)
        {
            
            if(catName!=null)//有選category才會傳入catName
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

        [HttpPost]
        public IActionResult CartEditQuantity(int newQuantity, int pId) 
        {
            string json = "";
            List<CCartItem> cart;
            json = HttpContext.Session.GetString(CDictionary.SK_PURCHASED_PRODUCTS_LIST);
            cart = JsonSerializer.Deserialize<List<CCartItem>>(json);
            var existingCartItem = cart.FirstOrDefault(item => item.cId == pId);
            existingCartItem.c數量 = newQuantity;
            // 將更新後的購物車列表序列化成 JSON，並存入 Session 變數中
            SaveCart(cart);

            return RedirectToAction("Cart");
        }

        [HttpPost]
        public IActionResult CartEditAttribute(string newAttribute, int pId)
        {
            string json = "";
            List<CCartItem> cart;
            json = HttpContext.Session.GetString(CDictionary.SK_PURCHASED_PRODUCTS_LIST);
            cart = JsonSerializer.Deserialize<List<CCartItem>>(json);
            var existingCartItem = cart.FirstOrDefault(item => item.cId == pId);

            existingCartItem.c子項目 = newAttribute;
            existingCartItem.cId= _productService.getIdFromAttribute(newAttribute);
            // 將更新後的購物車列表序列化成 JSON，並存入 Session 變數中
            SaveCart(cart);

            return RedirectToAction("Cart");
        }
        [HttpPost]
        public IActionResult AddToCart(int pId)
        {
            var prodItem = _productService.getProductById(pId);

            var cart = GetCartFromSession();


            // 調用簡化方法，傳入產品物件和數量
            _productService.addCartItem(cart, prodItem, 1);
            SaveCart(cart);

            //json = JsonSerializer.Serialize(cart);
            //HttpContext.Session.SetString(CDictionary.SK_PURCHASED_PRODUCTS_LIST, json);

            return RedirectToAction("Shop");
        }

        [HttpPost]
        public IActionResult DetailsAddToCart(int pId, string attr, int count)
        {
            
           
            var prodItem = _productService.getProductById(pId);
            // 接下來進行購物車處理...
            var cart = GetCartFromSession();


            // 調用簡化方法，傳入產品物件和數量
            _productService.detailsAddCartItem(cart, prodItem, attr, count);
            SaveCart(cart);


            return RedirectToAction("ShopDetail");

        }
        //[HttpPost]
        //public IActionResult DetailsAddToCart(int pId, string selected, int count)
        //{
        //    var prodItem = _productService.GetProductById(pId);

        //    var cart = GetCartFromSession();


        //    // 調用簡化方法，傳入產品物件和數量
        //    _productService.DetailsAddCartItem(cart, prodItem, selected, count);
        //    SaveCart(cart);

        //    //json = JsonSerializer.Serialize(cart);
        //    //HttpContext.Session.SetString(CDictionary.SK_PURCHASED_PRODUCTS_LIST, json);

        //    return RedirectToAction("ShopDetail");
        //}

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


        private void SaveCart(List<CCartItem> cart)
        {
            string json = JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString(CDictionary.SK_PURCHASED_PRODUCTS_LIST, json);
        }

        


        public IActionResult GetDetails(int? pId)
        {
            var details = _productService.getDetailsById(pId);

            return Json(details);
        }
    }
}
