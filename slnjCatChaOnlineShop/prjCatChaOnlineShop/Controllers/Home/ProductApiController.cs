using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using prjCatChaOnlineShop.Models;
using prjCatChaOnlineShop.Models.CDictionary;
using prjCatChaOnlineShop.Models.CModels;
using prjCatChaOnlineShop.Models.ViewModels;
using prjCatChaOnlineShop.Services.Function;
using System.Text.Json;

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
        public IActionResult ShopItemPerPage(int itemPerPage)
        {
            var items = _productService.GetProductItems().DistinctBy(item => item.product.ProductName).Take(itemPerPage);//只出現商品名稱不同的品項
            return Json(items);
        }

        public IActionResult AddToCart(int pId)
        {
            var prodItem=_productService.GetProductById(pId);
            string json = "";
            List<CCartItem> cart ;
            if (HttpContext.Session.Keys.Contains(CDictionary.SK_PURCHASED_PRODUCTS_LIST))
            {
                json = HttpContext.Session.GetString(CDictionary.SK_PURCHASED_PRODUCTS_LIST);
                cart = JsonSerializer.Deserialize<List<CCartItem>>(json);
                if (cart.FirstOrDefault(item => item.cId == pId)!=null)
                {
                    CCartItem cartItem = new CCartItem();
                    cartItem.c數量++;
                    // 商品已在購物車中只加數量
                    return RedirectToAction("Shop");
                }
            }
            else
            {
                cart = new List<CCartItem>();
                CCartItem cartItem = new CCartItem();
                cartItem.cId = pId;
                cartItem.cName = prodItem.product.ProductName;
                
                if (prodItem.pSalePrice!=null) //特價時的金額
                    cartItem.cPrice = prodItem.pSalePrice;

                cartItem.cPrice= prodItem.product.ProductPrice; //無特價時金額
                cartItem.cImgPath= prodItem.pImgPath;
                cartItem.c子選項 = prodItem.product.Attributes;
                cartItem.c數量 = 1;//預設點第一次加入一個
                cartItem.c剩餘庫存 = prodItem.product.RemainingQuantity;
                cart.Add(cartItem);
                json = JsonSerializer.Serialize(cart);
                HttpContext.Session.SetString(CDictionary.SK_PURCHASED_PRODUCTS_LIST, json);
            }
                
            return RedirectToAction("Shop");
        }
        //public IActionResult ShopSelectCategory(int itemPerPage)
        //{
            
        //    return Json();
        //}
    }
}
