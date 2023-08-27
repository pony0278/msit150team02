using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjCatChaOnlineShop.Areas.AdminCMS.Models;
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

        //載入DataTable資料
        public IActionResult LoadDataTable()
        {
            var data = _cachaContext.GameProductTotal.ToList();

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
            var editProduct = _cachaContext.GameProductTotal
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
            var insertImgList = _cachaContext.GameProductTotal
                                .Where(x => x.ProductId == cGameproduct.ProductId)
                                .ToList();

            List<string> imageUrls = new List<string>();
            if (cGameproduct.ProductPhotos != null && cGameproduct.ProductPhotos.Count > 0)
            {
                for (int i = 0; i < cGameproduct.ProductPhotos.Count; i++)
                {
                    try
                    {
                        var uploadedImageUrl = await _imageService.UploadImageAsync(cGameproduct.ProductPhotos[i]);
                        imageUrls.Add(uploadedImageUrl);

                        if (i < insertImgList.Count)
                        {
                            insertImgList[i].ProductImage = uploadedImageUrl;
                        }
                        else
                        {
                            var newImageItem = new GameProductTotal
                            {
                                ProductId = cGameproduct.ProductId,
                                ProductImage = uploadedImageUrl
                            };
                            _cachaContext.GameProductTotal.Add(newImageItem);
                            _cachaContext.SaveChanges();
                        }
                    }
                    catch
                    {
                        return BadRequest("圖片上傳錯誤.");
                    }
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

                if (cGameproduct.ProductCategory != null)
                    editProduct.ProductCategory = cGameproduct.ProductCategory;

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
        public async Task<IActionResult> CreateProduct(CGameProductWrap cProduct)
        {
            var NewProduct = new GameProductTotal
            {
                ProductId= cProduct.ProductId,
                ProductName = cProduct.ProductName,
                ProductDescription = cProduct.ProductDescription,
                ProductCategory = cProduct.ProductCategory,
                ProductPrice = cProduct.ProductPrice,
                PurchasedQuantity= cProduct.PurchasedQuantity,
                RemainingQuantity= cProduct.RemainingQuantity,
                LotteryProbability= cProduct.LotteryProbability
            };
            _cachaContext.GameProductTotal.Add(NewProduct);
            await _cachaContext.SaveChangesAsync();

            List<string> imageUrls = new List<string>();
            if (cProduct.ProductPhotos != null && cProduct.ProductPhotos.Count > 0)
            {
                for (var i = 0; i < cProduct.ProductPhotos.Count; i++)
                {
                    try
                    {
                        var uploadedImageUrl = await _imageService.UploadImageAsync(cProduct.ProductPhotos[i]);
                        var newImageItem = new GameProductTotal
                        {
                            ProductId = NewProduct.ProductId,
                            ProductImage = uploadedImageUrl
                        };
                        _cachaContext.GameProductTotal.Add(newImageItem);
                    }
                    catch
                    {
                        return BadRequest("圖片上傳錯誤.");
                    }
                }
                await _cachaContext.SaveChangesAsync();
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


        public IActionResult GameProduct()
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


    }
}
