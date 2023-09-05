using Azure.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using prjCatChaOnlineShop.Models;
using prjCatChaOnlineShop.Models.CModels;
using prjCatChaOnlineShop.Services.Function;
using System.Collections.Generic;
using System.Security.Cryptography;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace prjCatChaOnlineShop.Controllers.Home
{
    public class IndexController : Controller
    {
        private readonly cachaContext _context;
        private readonly ProductService _productService;

        public IndexController(cachaContext context, ProductService productService)
        {
            _context = context;
            _productService = productService;
        }
        public IActionResult ShopDetail(int pId)
        {
            string userName = HttpContext.Session.GetString("UserName");
            ViewBag.UserName = userName;//把使用者名字傳給_Layout
            ViewBag.Categories=_productService.getAllCategories();//把類別傳給_Layout
            var details = _productService.getDetailsById(pId);

            return View(details);
        }

        public IActionResult Shop()
        {
            var items = _productService.getProductItems();

            string userName = HttpContext.Session.GetString("UserName");
            ViewBag.UserName = userName;//把使用者名字傳給_Layout
            ViewBag.ProductCount= items.Count();
            ViewBag.Categories = _productService.getAllCategories();//把類別傳給_Layout
            ViewBag.CategoryName = Request.Query["categoryName"];
            ViewBag.OrderBy = Request.Query["orderBy"];
            return View(items);
        }
        public IActionResult Index()
        {
            var banners = _context.GameShopBanner.Where(b => b.Display == true).ToList();
            var productItems = banners.Select(b => new CProductItem { Link = b.Link, Display = b.Display }).ToList();

            var items = _productService.getProductItems();
            string userName = HttpContext.Session.GetString("UserName");
            ViewBag.UserName = userName;
            ViewBag.Categories = _productService.getAllCategories();
            ViewBag.Banners = productItems;

            return View(items);
        }
        [HttpGet]
        public IActionResult CountDownProduct()
        {
            var data = _context.ShopProductTotal
                .Where(x => x.PushToShop == true && x.Discontinued == false)
                .Select(x => new
                {
                    ProductId = x.ProductId,
                    ProductName = x.ProductName,
                    Discount = x.Discount,
                    OffDay = x.OffDay,
                    PushToShop = x.PushToShop,
                }).ToList();
            return Json(new { data });
        }
        [HttpGet]
        public IActionResult GetReivews()
        {
            var data = _context.ShopProductReviewTable
                .Where(x=>x.ProductRating == 5 && x.HideReview != true || x.HideReview == null)
                .Take(5).Select(x=> new {
                    productRating = x.ProductRating,
                    productID = x.ProductId,
                    productName = x.Product.ProductName,
                    productdescription = x.ReviewContent,
                    Member = x.Member.Name.Length > 2? 
                             x.Member.Name.Substring(0 , x.Member.Name.Length -2) + "*"+"*":
                             x.Member.Name,
                    productPhoto = x.Product.ShopProductImageTable.FirstOrDefault().ProductPhoto,
                }).ToList();
            return Json(new { data });
        }
    }
}
