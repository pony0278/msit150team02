using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using prjCatChaOnlineShop.Areas.AdminCMS.Models.ViewModels;
using prjCatChaOnlineShop.Models;
using prjCatChaOnlineShop.Models.CModels;
using prjCatChaOnlineShop.Models.ViewModels;

namespace prjCatChaOnlineShop.Controllers.CMS
{
    [Area("AdminCMS")]
    public class ProductReviewController : Controller
    {
        private readonly cachaContext? _context;
        public ProductReviewController(cachaContext context ) 
        {
            _context = context;
        }

        public IActionResult ProductReview()
        {
            var ReviewModel = new CproductsReviews
            {
                reviewTables = _context.ShopProductReviewTable.ToList(),
                productCategories = _context.ShopProductCategory.ToList(),
            };
            return View(ReviewModel);
        }
        [HttpGet]
        public IActionResult tableData()
        {
            var rawdata = _context.ShopProductReviewTable
                            .Include(x=>x.Member)
                            .Include(x=>x.Product)
                            .ThenInclude(x=>x.ProductCategory)
                            .ToList();
            var data = rawdata.Select(x =>
            {
                return new
                {
                    MemberId = x.MemberId,
                    MemberName = x.Member?.Name,
                    ProductId = x.ProductId,
                    ProductName = x.Product?.ProductName,
                    productcatgory = x.Product?.ProductCategory.CategoryName,
                    ReviewContent = x.ReviewContent,
                    ProductRating = x.ProductRating,
                    ProductReviewId = x.ProductReviewId,
                    ReviewTime = x.ReviewTime == null? "未設定": x.ReviewTime.ToString(),
                    HideReview = x.HideReview == null ? "未設定" :
                                 x.HideReview == true ? "是" : "否",
                };
            }).ToList();
            return Json(new { data});
        }
        public IActionResult Delete(int? id)
        {
            if(id != null)
            {
                ShopProductReviewTable ProductReviews = _context.ShopProductReviewTable.FirstOrDefault(x => x.ProductReviewId == id);
                if(ProductReviews != null)
                {
                    _context.ShopProductReviewTable.Remove(ProductReviews);
                    _context.SaveChanges();
                }
            }
            return RedirectToAction("ProductReview", " ProductReview", new { area = "AdminCMS" });
        }
        [HttpGet]
        public IActionResult EditorReview(int? id)
        {
            ShopProductReviewTable ProductReviews = _context.ShopProductReviewTable
                                                    .Include(x=>x.Member)
                                                    .Include(x=>x.Product)
                                                    .ThenInclude(x=>x.ProductCategory)
                                                    .FirstOrDefault(x => x.ProductReviewId == id);
            if(ProductReviews == null)
            {
                return Json(new { success = false, message = "Item not found" });
            }
            string json = JsonConvert.SerializeObject(new { success = true, data = ProductReviews },
                new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

            return Content(json, "application/json");
        }
        [HttpPost]
        public IActionResult EditorReviews([FromBody] CProductReviewsWrap cProductReviews) // 使用 [FromBody]
        {
            ShopProductReviewTable shopProductReview = _context.ShopProductReviewTable.FirstOrDefault(x => x.ProductReviewId == 6);
            if (shopProductReview != null)
            {
                shopProductReview.ProductRating = cProductReviews.ProductRating;
                _context.Update(shopProductReview);
                _context.SaveChanges();
                return RedirectToAction("news", "News", new { area = "AdminCMS" });
            }
            return RedirectToAction("news", "News", new { area = "AdminCMS" });
        }
        [HttpPost]
        public IActionResult editor([FromBody] int ProductReviewId)
        {
            ShopProductReviewTable ProductReviews = _context.ShopProductReviewTable.FirstOrDefault(x => x.ProductReviewId == ProductReviewId);
            return Json(new { success = true, message = "Item updated successfully" });
        }




    }
}
