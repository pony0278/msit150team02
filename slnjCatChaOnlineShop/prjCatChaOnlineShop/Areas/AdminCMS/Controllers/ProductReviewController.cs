using Microsoft.AspNetCore.Mvc;
using prjCatChaOnlineShop.Models;

namespace prjCatChaOnlineShop.Controllers.CMS
{
    [Area("AdminCMS")]
    public class ProductReviewController : Controller
    {
        private readonly cachaContext? _context;
       
        public ProductReviewController(cachaContext context) 
        {
            _context = context;
        }

        public IActionResult ProductReview()
        {
            return View();
        }
        public IActionResult tableData()
        {
            return View();
        }
    }
}
