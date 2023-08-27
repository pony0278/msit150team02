using Microsoft.AspNetCore.Mvc;

namespace prjCatChaOnlineShop.Controllers
{
    public class GameController : SuperController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
