using Azure.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using prjCatChaOnlineShop.Models;
using prjCatChaOnlineShop.Models.CModels;
using prjCatChaOnlineShop.Services.Function;
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
            var details = _productService.getDetailsById(pId);
            return View(details);
        }

        public IActionResult Shop()
        {
            return View(_productService.getProductItems());
        }
        public IActionResult Index()
        {
            return View(_productService.getProductItems());
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
    }
}
