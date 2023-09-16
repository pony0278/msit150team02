using Microsoft.AspNetCore.Mvc;
using prjCatChaOnlineShop.Models;
using prjCatChaOnlineShop.Services.Function;

namespace prjCatChaOnlineShop.Controllers.Home
{
    public class AboutController : Controller
    {
        private readonly ProductService _productService;

        public AboutController(ProductService productService)
        {
            _productService = productService;
        }
        public IActionResult About()
        {
            string userName = HttpContext.Session.GetString("UserName");
            ViewBag.UserName = userName;//把使用者名字傳給_Layout
            ViewBag.Categories = _productService.getAllCategories();//把類別傳給_Layout


            return View();
        }
        public IActionResult Index()
        {
            string userName = HttpContext.Session.GetString("UserName");
            ViewBag.UserName = userName;
            ViewBag.Categories = _productService.getAllCategories();//把類別傳給_Layout

            return View();
        }
        public IActionResult B()
        {
            return View();
        }
    }
}
