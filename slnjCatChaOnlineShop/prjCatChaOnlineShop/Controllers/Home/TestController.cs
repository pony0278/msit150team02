using Microsoft.AspNetCore.Mvc;
using prjCatChaOnlineShop.Models;
using prjCatChaOnlineShop.Services.Function;

namespace prjCatChaOnlineShop.Controllers.Home
{
    public class TestController : Controller
    {
        private readonly ImageService _imageService;
        private readonly cachaContext _cachaContext;

        public TestController(ImageService imageService, cachaContext cachaContext)
        {
            _imageService = imageService;
            _cachaContext = cachaContext;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile image)
        {
            var imageUrl = await _imageService.UploadImageAsync(image);
            ViewBag.ImageUrl = imageUrl;
            return RedirectToAction("Index");
        }

    }
}
