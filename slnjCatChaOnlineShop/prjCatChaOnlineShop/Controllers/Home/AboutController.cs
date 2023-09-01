using Microsoft.AspNetCore.Mvc;

namespace prjCatChaOnlineShop.Controllers.Home
{
    public class AboutController : Controller
    {
        public IActionResult About()
        {
            string userName = HttpContext.Session.GetString("UserName");
            ViewBag.UserName = userName;//把使用者名字傳給_Layout

            return View();
        }
        public IActionResult Index()
        {
            string userName = HttpContext.Session.GetString("UserName");
            ViewBag.UserName = userName;
            return View();
        }
    }
}
