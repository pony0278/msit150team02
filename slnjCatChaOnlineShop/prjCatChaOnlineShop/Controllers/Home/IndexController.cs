using Microsoft.AspNetCore.Mvc;
using prjCatChaOnlineShop.Models;
using prjCatChaOnlineShop.Services.Function;

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
        public IActionResult ShopDetail()
        {
            return View();
        }
        public IActionResult Shop()
        {
            return View(_productService.GetProductItems());
        }
        public IActionResult Index()
        {
            return View(_productService.GetProductItems());
        }
    }
}
