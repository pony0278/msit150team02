using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using prjCatChaOnlineShop.Areas.AdminCMS.Models;
using prjCatChaOnlineShop.Areas.AdminCMS.Models.ViewModels;
using prjCatChaOnlineShop.Models;
using prjCatChaOnlineShop.Models.CModels;
using prjCatChaOnlineShop.Models.ViewModels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
            //判斷是否有登入
            if (HttpContext.Session.Keys.Contains(CAdminLogin.SK_LOGINED_USER))
            {
                // 讀取管理員姓名
                string adminName = HttpContext.Session.GetString("AdminName");

                // 將管理員姓名傳給view
                ViewBag.AdminName = adminName;

                var ReviewModel = new CproductsReviews
                {
                    reviewTables = _context.ShopProductReviewTable.ToList(),
                    productCategories = _context.ShopProductCategory.ToList(),
                };
                return View(ReviewModel);
            }
            return RedirectToAction("Login", "CMSHome");
            
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
        public IActionResult editor([FromBody] CProductReviewsWrap reviewData)
        {
            ShopProductReviewTable shopProductReview = _context.ShopProductReviewTable.FirstOrDefault(x => x.ProductReviewId == reviewData.ProductReviewId);
            if (shopProductReview != null)
            {
                shopProductReview.HideReview = reviewData.HideReview;
                _context.Update(shopProductReview);
                _context.SaveChanges();
                return Json(new { success = true, message = "Item updated successfully" });
            }
            return Json(new { success = false, message = "Item not found" });
        }
        [HttpGet]
        public IActionResult queryReviews(int? productType , int? rating , bool? hide)
        {
            var query = _context.ShopProductReviewTable.AsQueryable();

            if (productType.HasValue)
            {
                query = query.Where(x => x.Product.ProductCategory.ProductCategoryId == productType.Value);
            }

            if (rating.HasValue)
            {
                query = query.Where(x => x.ProductRating == rating.Value);
            }

            if (hide.HasValue)
            {
                query = query.Where(x => x.HideReview == hide.Value);
            }

            var queryResult = query
                  .Include(x => x.Member)
                  .Include(x => x.Product)
                      .ThenInclude(x => x.ProductCategory)
                  .ToList();

            var data = queryResult.Select(x => new
            {
                MemberId = x.MemberId,
                MemberName = x.Member == null ? null : x.Member.Name,
                ProductId = x.ProductId,
                ProductName = x.Product == null ? null : x.Product.ProductName,
                productcatgory = x.Product == null ? null : x.Product.ProductCategory.CategoryName,
                ReviewContent = x.ReviewContent,
                ProductRating = x.ProductRating,
                ProductReviewId = x.ProductReviewId,
                ReviewTime = x.ReviewTime?.ToString() ?? "未設定",
                HideReview = x.HideReview == null ? "未設定" :
                                 x.HideReview == true ? "是" : "否",
            }).ToList();

            return Json(new { data });
        }
   




    }
}
