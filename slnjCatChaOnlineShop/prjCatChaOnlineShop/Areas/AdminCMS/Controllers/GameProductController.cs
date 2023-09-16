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
                LotteryProbability = x.LotteryProbability
            }).ToList();
            return Json(new { data });
        }

        //編輯商品資料
        [HttpGet]
        public IActionResult EditProduct(int id)
        {
            try
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
            catch (Exception ex)
            {
                return Json(new { success = false, message = "出現了一個錯誤：" + ex.Message });
            }
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
            try
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
                    catch (Exception ex)
                    {
                        return BadRequest("圖片上傳錯誤: " + ex.Message);
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

                    if (cGameproduct.LotteryProbability != null)
                        editProduct.LotteryProbability = cGameproduct.LotteryProbability;

                    _cachaContext.Update(editProduct);
                    _cachaContext.SaveChanges();
                    return Json(new { success = true, message = "商品已成功更新" });
                }
                return Json(new { success = false, message = "商品未找到" });
            }
            catch (Exception ex)
            {
                return BadRequest("出現了一個錯誤：" + ex.Message);
            }
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
                    try
                    {
                        _cachaContext.GameProductTotal.Remove(cProduct);
                        _cachaContext.SaveChanges();
                        return Json(new { success = true });
                    }
                    catch { return BadRequest(); }
                }
            }
            return Json(new { success = false });
        }

        //發送給會員
        [HttpPost]
        public IActionResult SendProductsToMemberByID(int[] ids, int? memberId, int quantity)
        {
            try
            {
                if (quantity <= 0)
                {
                    // 驗證數量必須大於零
                    return Json(new { success = false, message = "商品數量必須大於零" });
                }

                foreach (int id in ids)
                {
                    GameProductTotal product = _cachaContext.GameProductTotal.FirstOrDefault(p => p.ProductId == id);

                    if (product == null)
                    {
                        return Json(new { success = false, message = "商品不存在" });
                    }

                    ShopMemberInfo member = _cachaContext.ShopMemberInfo.FirstOrDefault(m => m.MemberId == memberId);

                    if (member == null)
                    {
                        return Json(new { success = false, message = "會員不存在" });
                    }

                    // 創建會員優惠券資料
                    // 查詢資料庫以獲取當前數量
                    var existingRecord = _cachaContext.GameItemPurchaseRecord
                        .SingleOrDefault(r => r.MemberId == memberId.Value && r.ProductId == product.ProductId);

                    if (existingRecord != null)
                    {
                        // 如果記錄存在，則遞增數量
                        existingRecord.QuantityOfInGameItems += quantity;
                    }
                    else
                    {
                        // 如果記錄不存在，則創建新記錄
                        GameItemPurchaseRecord item = new GameItemPurchaseRecord
                        {
                            MemberId = member.MemberId,
                            ProductId = product.ProductId,
                            QuantityOfInGameItems = quantity  // 使用輸入的數量
                        };
                        _cachaContext.GameItemPurchaseRecord.Add(item);
                    }
                }

                // 將更改保存到資料庫
                _cachaContext.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // 在這裡處理異常，可以記錄錯誤、返回自定義錯誤消息，或者進行其他適當的處理
                return Json(new { success = false, message = "出現了一個錯誤：" + ex.Message });
            }
        }
    }
}
