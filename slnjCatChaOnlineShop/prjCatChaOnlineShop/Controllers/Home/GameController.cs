using Microsoft.AspNetCore.Mvc;
using prjCatChaOnlineShop.Controllers.Api;

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
