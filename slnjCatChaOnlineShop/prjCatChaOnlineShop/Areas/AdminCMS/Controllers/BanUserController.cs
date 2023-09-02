using prjCatChaOnlineShop.Models;
using prjCatChaOnlineShop.Models.CModels;
using prjCatChaOnlineShop.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace prjCatChaOnlineShop.Areas.AdminCMS.Controllers
{
    [Area("AdminCMS")]
    public class BanUserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
