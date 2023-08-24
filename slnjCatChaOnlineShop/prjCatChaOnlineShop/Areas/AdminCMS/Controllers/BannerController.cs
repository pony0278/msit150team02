using Microsoft.AspNetCore.Mvc;
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
            return View();
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
