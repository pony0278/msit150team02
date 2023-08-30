using Microsoft.AspNetCore.Mvc;

namespace prjCatChaOnlineShop.Areas.AdminCMS.Controllers
{
    public class NewsletterController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
