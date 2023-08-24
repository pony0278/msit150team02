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
        public IActionResult AddToCart(int pId)
        {
            var prodItem = _productService.GetProductById(pId);

            // 檢查 Session 中是否已存在購物車
            string json = "";
            List <CCartItem> cart;
            if (HttpContext.Session.Keys.Contains(CDictionary.SK_PURCHASED_PRODUCTS_LIST))
            {
                json = HttpContext.Session.GetString(CDictionary.SK_PURCHASED_PRODUCTS_LIST);
                cart = JsonSerializer.Deserialize<List<CCartItem>>(json);

                // 檢查是否已存在相同的商品在購物車中
                var existingCartItem = cart.FirstOrDefault(item => item.cId == pId);
                if (existingCartItem != null)
                {
                    // 商品數量加一
                    existingCartItem.c數量++;
                }
                else
                {
                    // 創建新的購物車項目
                    CCartItem cartItem = new CCartItem();
                    cartItem.cId = pId;
                    cartItem.cName = prodItem.pName;

                    if (prodItem.pSalePrice != null)
                    {
                        cartItem.cPrice = prodItem.pSalePrice;//特價時的金額
                    }
                    else
                    {
                        cartItem.cPrice = prodItem.pPrice;//無特價時金額
                    }
                    cartItem.cImgPath = prodItem.pImgPath;
                    cartItem.c子選項 = prodItem.p子項目;
                    cartItem.c剩餘庫存 = prodItem.p剩餘庫存;

                    // 初次點擊，設置商品數量為 1
                    cartItem.c數量 = 1;

                    // 將購物車項目加入購物車列表
                    cart.Add(cartItem);
                }
            }
            else
            {
                cart = new List<CCartItem>();
                CCartItem cartItem = new CCartItem();
                cartItem.cId = pId;
                cartItem.cName = prodItem.pName;

                if (prodItem.pSalePrice != null)
                {
                    cartItem.cPrice = prodItem.pSalePrice;//特價時的金額
                }
                else
                {
                    cartItem.cPrice = prodItem.pPrice;//無特價時金額
                }
                cartItem.cImgPath = prodItem.pImgPath;
                cartItem.c子選項 = prodItem.p子項目;
                cartItem.c剩餘庫存 = prodItem.p剩餘庫存;

                // 初次點擊，設置商品數量為 1
                cartItem.c數量 = 1;

                // 將購物車項目加入購物車列表
                cart.Add(cartItem);
            }

            // 將更新後的購物車列表序列化成 JSON，並存入 Session 變數中
            json = JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString(CDictionary.SK_PURCHASED_PRODUCTS_LIST, json);

            return RedirectToAction("Shop");
        }

    }
}
