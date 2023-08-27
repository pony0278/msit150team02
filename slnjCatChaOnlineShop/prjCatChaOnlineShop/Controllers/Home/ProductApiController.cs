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
                var items = _productService.GetProductByCategoryName(catName).Take(itemPerPage);
                var name = items.FirstOrDefault()?.pCategoryName;
                CCategoryItem c = new CCategoryItem();
                c.pItem = items.ToList();
                c.categoryName = name;
                categoryItems.Add(c);
                return Json(categoryItems);
            }
            else
            {
                var allItems = _productService.GetProductItems().Take(itemPerPage);//只出現商品名稱不同的品項
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
        public IActionResult AddToCart(int pId)
        {
            var prodItem = _productService.GetProductById(pId);

            var cart = GetCartFromSession();


            // 調用簡化方法，傳入產品物件和數量
            _productService.AddCartItem(cart, prodItem, 1);
            SaveCart(cart);

            //json = JsonSerializer.Serialize(cart);
            //HttpContext.Session.SetString(CDictionary.SK_PURCHASED_PRODUCTS_LIST, json);

            return RedirectToAction("Shop");
        }

        [HttpPost]
        public IActionResult DetailsAddToCart(CDetailsViewModel vm)
        {
            var prodItem = _productService.GetProductById(vm.selectedProduct.pId);

            var cart = GetCartFromSession();


            // 調用簡化方法，傳入產品物件和數量
            _productService.AddCartItem(cart, prodItem, vm.txtCount);
            SaveCart(cart);

            //json = JsonSerializer.Serialize(cart);
            //HttpContext.Session.SetString(CDictionary.SK_PURCHASED_PRODUCTS_LIST, json);

            return RedirectToAction("ShopDetail");
        }


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

        //[HttpPost]
        //public IActionResult AddToCart(int pId)
        //{
        //    var prodItem = _productService.GetProductById(pId);

        //    // 檢查 Session 中是否已存在
        //    string json = "";
        //    List <CCartItem> cart;
        //    if (HttpContext.Session.Keys.Contains(CDictionary.SK_PURCHASED_PRODUCTS_LIST))
        //    {
        //        json = HttpContext.Session.GetString(CDictionary.SK_PURCHASED_PRODUCTS_LIST);
        //        cart = JsonSerializer.Deserialize<List<CCartItem>>(json);

        //        // 檢查是否已存在相同的商品在購物車中
        //        var existingCartItem = cart.FirstOrDefault(item => item.cId == pId);
        //        if (existingCartItem != null)
        //        {
        //            //若還有庫存商品數量加一
        //            if (existingCartItem.c剩餘庫存 >= 1)
        //                existingCartItem.c數量++;
        //        }
        //        else
        //        {
        //            // 創建新的購物車項目
        //            CCartItem cartItem = new CCartItem();
        //            cartItem.cId = pId;
        //            cartItem.cName = prodItem.pName;
        //            cartItem.cPrice = _productService.price(prodItem.pPrice, prodItem.p優惠價格);
        //            cartItem.cImgPath = prodItem.p圖片路徑;
        //            cartItem.c子項目 = prodItem.p子項目;
        //            cartItem.c剩餘庫存 = prodItem.p剩餘庫存;

        //            // 初次點擊，設置商品數量為 1
        //            cartItem.c數量 = 1;

        //            // 將購物車項目加入購物車列表
        //            cart.Add(cartItem);
        //        }
        //    }
        //    else
        //    {
        //        cart = new List<CCartItem>();
        //        CCartItem cartItem = new CCartItem();
        //        cartItem.cId = pId;
        //        cartItem.cName = prodItem.pName;
        //        cartItem.cPrice = _productService.price(prodItem.pPrice, prodItem.p優惠價格);
        //        //if (prodItem.p優惠價格 != null)
        //        //{
        //        //    cartItem.cPrice = prodItem.p優惠價格;//特價時的金額
        //        //}
        //        //else
        //        //{
        //        //    cartItem.cPrice = prodItem.pPrice;//無特價時金額
        //        //}
        //        cartItem.cImgPath = prodItem.p圖片路徑;
        //        cartItem.c子項目 = prodItem.p子項目;
        //        cartItem.c剩餘庫存 = prodItem.p剩餘庫存;

        //        // 初次點擊，設置商品數量為 1
        //        cartItem.c數量 = 1;
        //        cartItem.c其他子項目 = _productService.GetOtherAttr(pId);
        //        // 將購物車項目加入購物車列表
        //        cart.Add(cartItem);
        //    }

        //    // 將更新後的購物車列表序列化成 JSON，並存入 Session 變數中
        //    json = JsonSerializer.Serialize(cart);
        //    HttpContext.Session.SetString(CDictionary.SK_PURCHASED_PRODUCTS_LIST, json);

        //    return RedirectToAction("Shop");
        //}


        //[HttpPost]
        //public IActionResult DetailsAddToCart(CDetailsViewModel vm)
        //{
        //    var prodItem = _productService.GetProductById(vm.selectedProduct.pId);

        //    // 檢查 Session 中是否已存在購物車
        //    string json = "";
        //    List<CCartItem> cart;
        //    if (HttpContext.Session.Keys.Contains(CDictionary.SK_PURCHASED_PRODUCTS_LIST))
        //    {
        //        json = HttpContext.Session.GetString(CDictionary.SK_PURCHASED_PRODUCTS_LIST);
        //        cart = JsonSerializer.Deserialize<List<CCartItem>>(json);

        //        // 檢查是否已存在相同的商品在購物車中
        //        var existingCartItem = cart.FirstOrDefault(item => item.cId == vm.selectedProduct.pId);
        //        if (existingCartItem != null)
        //        {
        //            //若還有庫存商品數量加一
        //            if (existingCartItem.c剩餘庫存 >= 1)
        //                existingCartItem.c數量++;
        //        }
        //        else
        //        {
        //            // 創建新的購物車項目
        //            CCartItem cartItem = new CCartItem();
        //            cartItem.cId = vm.selectedProduct.pId;
        //            cartItem.cName = vm.selectedProduct.pName;
        //            cartItem.cPrice = _productService.price(prodItem.pPrice, prodItem.p優惠價格);
        //            cartItem.cImgPath = vm.selectedProduct.p圖片路徑;
        //            cartItem.c子項目 = vm.selectedProduct.p子項目;
        //            cartItem.c剩餘庫存 = vm.selectedProduct.p剩餘庫存;

        //            // 初次點擊，設置商品數量為 1
        //            cartItem.c數量 = vm.txtCount;

        //            // 將購物車項目加入購物車列表
        //            cart.Add(cartItem);
        //        }
        //    }
        //    else
        //    {
        //        cart = new List<CCartItem>();
        //        CCartItem cartItem = new CCartItem();
        //        cartItem.cId = vm.selectedProduct.pId;
        //        cartItem.cName = vm.selectedProduct.pName;
        //        cartItem.cPrice = _productService.price(prodItem.pPrice, prodItem.p優惠價格);
        //        cartItem.cImgPath = vm.selectedProduct.p圖片路徑;
        //        cartItem.c子項目 = vm.selectedProduct.p子項目;
        //        cartItem.c剩餘庫存 = vm.selectedProduct.p剩餘庫存;

        //        // ShopDetail可輸入數量
        //        cartItem.c數量 = vm.txtCount;

        //        // 將購物車項目加入購物車列表
        //        cart.Add(cartItem);
        //    }
        //    // 將更新後的購物車列表序列化成 JSON，並存入 Session 變數中
        //    json = JsonSerializer.Serialize(cart);
        //    HttpContext.Session.SetString(CDictionary.SK_PURCHASED_PRODUCTS_LIST, json);

        //    return RedirectToAction("ShopDetail");
        //}


        public IActionResult GetDetails(int? pId)
        {
            var details = _productService.GetDetailsById(pId);

            return Json(details);
        }
    }
}
