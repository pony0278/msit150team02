using Microsoft.AspNetCore.Mvc;
using prjCatChaOnlineShop.Areas.AdminCMS.Models;
using prjCatChaOnlineShop.Models;
using prjCatChaOnlineShop.Services.Function;

namespace prjCatChaOnlineShop.Controllers.CMS
{
    [Area("AdminCMS")]
    public class BannerController : Controller
    {
        private readonly ImageService _imageService;
        private readonly cachaContext _cachaContext;
        public BannerController(ImageService imageService, cachaContext cachaContext)
        {
            _cachaContext = cachaContext;
            _imageService = imageService;
        }

        public IActionResult Banner()
        {
            //判斷是否有登入
            if (HttpContext.Session.Keys.Contains(CAdminLogin.SK_LOGINED_USER))
            {
                // 讀取管理員姓名
                string adminName = HttpContext.Session.GetString("AdminName");

                // 將管理員姓名傳給view
                ViewBag.AdminName = adminName;

                return View();
            }
            return RedirectToAction("Login", "CMSHome");
        }

        public IActionResult LoadDataTable()
        {
            var data = _cachaContext.GameShopBanner;

            return Json(new { data });


            //    var data = _cachaContext.GameProductTotal.Select(x => new
            //    {





            //    }).ToList();

            //    return Json(new { data });
        }

    }
}
