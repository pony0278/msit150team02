using Microsoft.AspNetCore.Mvc;
using prjCatChaOnlineShop.Models;
using prjCatChaOnlineShop.Services.Function;

namespace prjCatChaOnlineShop.Controllers
{
    public class GameController : SuperController
    {
        private readonly ProductService _productService;
        public GameController(ProductService productService)
        {
            _productService = productService;
        }
        public IActionResult Index()
        {
            string userName = HttpContext.Session.GetString("UserName");
            ViewBag.Categories = _productService.getAllCategories();//把類別傳給_Layout
            ViewBag.UserName = userName;//把使用者名字傳給_Layout
            return View();
        }
    }
}
