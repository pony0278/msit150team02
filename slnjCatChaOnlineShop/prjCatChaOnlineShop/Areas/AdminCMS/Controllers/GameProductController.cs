using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Experimental.ProjectCache;
using Microsoft.EntityFrameworkCore;
using prjCatChaOnlineShop.Areas.AdminCMS.Models;
using prjCatChaOnlineShop.Areas.AdminCMS.Models.ViewModels;
using prjCatChaOnlineShop.Models;
using prjCatChaOnlineShop.Models.ViewModels;
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
            //判斷是否有登入
            if (HttpContext.Session.Keys.Contains(CAdminLogin.SK_LOGINED_USER))
            {
                // 讀取管理員姓名
                string adminName = HttpContext.Session.GetString("AdminName");

                // 將管理員姓名傳給view
                ViewBag.AdminName = adminName;

                var product = new GameProductVM
                {
                    GameProductCategory = _cachaContext.GameProductCategory.ToList(),
                    //GameProductTotal = _cachaContext.GameProductTotal.ToList(),
                };
                return View(product);
            }
            return RedirectToAction("Login", "CMSHome");
        }

        //載入DataTable資料
        public IActionResult LoadDataTable()
        {
            var data = _cachaContext.GameProductTotal.Select(x => new
            {
                ProductId = x.ProductId,
                ProductName = x.ProductName,
                ProductCategoryId = x.ProductCategory.CategoryName,
                ProductDescription = x.ProductDescription,
                ProductPrice = x.ProductPrice,
                ProductImage = x.ProductImage,
                PurchasedQuantity = x.PurchasedQuantity,
                RemainingQuantity = x.RemainingQuantity,
                LotteryProbability = x.LotteryProbability
            }).ToList();
            return Json(new { data });
        }

        //編輯商品資料
        [HttpGet]
        public IActionResult EditProduct(int id)
        {

            if (id == null)
            {
                return Json(new { success = false, message = "ID不存在" });
            }
            GameProductTotal product = _cachaContext.GameProductTotal
                                                                    .Include(p => p.ProductCategory)
                                                                    .FirstOrDefault(p => p.ProductId == id);

            if (product == null)
            {
                return Json(new { success = false, message = "商品不存在" });
            }
            return Json(new { success = true, data = product });

        }

        //上傳圖片
        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                return BadRequest("未選擇圖片");
            }

            string imageUrl;
            try
            {
                imageUrl = await _imageService.UploadImageAsync(image);
            }
            catch
            {
                return BadRequest("圖片上傳失敗");
            }
            return Ok(new { imageUrl = $"{imageUrl}" });
        }

        //編輯後儲存商品資料
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> EditProduct([FromForm] CGameProductWrap cGameproduct)
        {
            var image = cGameproduct.Image;
            GameProductTotal editProduct = _cachaContext.GameProductTotal
                .FirstOrDefault(p => p.ProductId == cGameproduct.ProductId);

            string imageURL = null;
            if (image != null && image.Length > 0)
            {
                try
                {
                    imageURL = await _imageService.UploadImageAsync(image);
                }
                catch
                {
                    return BadRequest("圖片上傳錯誤.");
                }
            }


            if (editProduct != null)
            {
                if (imageURL != null)
                    editProduct.ProductImage = imageURL;

                if (cGameproduct.ProductName != null)
                    editProduct.ProductName = cGameproduct.ProductName;

                if (cGameproduct.ProductDescription != null)
                    editProduct.ProductDescription = cGameproduct.ProductDescription;

                if (cGameproduct.ProductCategoryId != null)
                    editProduct.ProductCategoryId = cGameproduct.ProductCategoryId;

                if (cGameproduct.ProductPrice != null)
                    editProduct.ProductPrice = cGameproduct.ProductPrice;

                if (cGameproduct.PurchasedQuantity != null)
                    editProduct.PurchasedQuantity = cGameproduct.PurchasedQuantity;

                if (cGameproduct.RemainingQuantity != null)
                    editProduct.RemainingQuantity = cGameproduct.RemainingQuantity;

                if (cGameproduct.LotteryProbability != null)
                    editProduct.LotteryProbability = cGameproduct.LotteryProbability;

                _cachaContext.Update(editProduct);
                _cachaContext.SaveChanges();
                 return Json(new { success = true, message = "Item updated successfully" });
            }
            return Json(new { success = false, message = "Item not found" });
        }

        //新增

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromForm] CGameProductWrap cGameproduct)
        {
            var image = cGameproduct.Image;

            string imageURL = null;
            if (image != null && image.Length > 0)
            {
                try
                {
                    imageURL = await _imageService.UploadImageAsync(image);
                }
                catch
                {
                    return BadRequest("圖片上傳錯誤.");
                }
            }

            var newProduct = new GameProductTotal
            {

                ProductImage = imageURL,
                ProductId = cGameproduct.ProductId,
                ProductName = cGameproduct.ProductName,
                ProductDescription = cGameproduct.ProductDescription,
                ProductCategoryId = cGameproduct.ProductCategoryId,
                ProductPrice = cGameproduct.ProductPrice,
                PurchasedQuantity = cGameproduct.PurchasedQuantity,
                RemainingQuantity = cGameproduct.RemainingQuantity,
                LotteryProbability = cGameproduct.LotteryProbability,

            };
            try
            {
                _cachaContext.GameProductTotal.Add(newProduct);
                await _cachaContext.SaveChangesAsync();
            }
            catch
            {
                return BadRequest("Error saving the product.");
            }
            return Json(new { success = true, message = "Content saved!" });
        }


        //刪除
        [HttpPost]
        public IActionResult Delete(int? id)
        {
            if (id != null)
            {
                GameProductTotal cProduct = _cachaContext.GameProductTotal.FirstOrDefault(p => p.ProductId == id);
                if (cProduct != null)
                {
                    _cachaContext.GameProductTotal.Remove(cProduct);
                    _cachaContext.SaveChanges();
                    return Json(new { success = true });
                }
            }
            return Json(new { success = false });
        }

    //發送給單一會員
    [HttpPost]
    public IActionResult SendCouponToMembersByID(int? id, int? memberId)
    {
        ShopCouponTotal coupon = _cachaContext.ShopCouponTotal.FirstOrDefault(c => c.CouponId == id);

        if (coupon == null)
        {
            return NotFound();
        }

        if (memberId == null)
        {
            return NotFound();
        }


        // 創建會員優惠券資料

        //TODO  寵物儲存資料表?

        //ShopMemberInfo memberInfo = new ShopMemberInfo
        //{
        //    MemberId = memberId.Value,

        //};

        //_cachaContext.ShopMemberCouponData.Add(memberCouponData);
        //_cachaContext.SaveChanges();

        return Json(new { success = true });
    }
}


}
