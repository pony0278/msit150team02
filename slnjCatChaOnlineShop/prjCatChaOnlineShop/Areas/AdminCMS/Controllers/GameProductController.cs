using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjCatChaOnlineShop.Models;
using prjCatChaOnlineShop.Services.Function;

namespace prjCatChaOnlineShop.Controllers.CMS
{
    [Area("AdminCMS")]
    public class GameProductController : Controller
    {
        private readonly ImageService _imageService;
        private readonly cachaContext _cachaContext;

        public GameProductController(ImageService imageService, cachaContext cachaContext)
        {
            _imageService = imageService;
            _cachaContext = cachaContext;
        }
        public IActionResult GameProduct()
        {
            return View();
        }


        //載入DataTable資料
        public IActionResult LoadDataTable()
        {
            var data = _cachaContext.GameProductTotal;

            return Json(new { data });


            //    var data = _cachaContext.GameProductTotal.Select(x => new
            //    {





            //    }).ToList();

            //    return Json(new { data });
        }





    }
}
