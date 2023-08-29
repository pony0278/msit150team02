using Microsoft.AspNetCore.Mvc;

namespace prjCatChaOnlineShop.Controllers
{
    public class GameController : SuperController
    {
        public IActionResult Index()
        {
            string userName = HttpContext.Session.GetString("UserName");
            ViewBag.UserName = userName;//把使用者名字傳給_Layout
            return View();
        }
    }
}
