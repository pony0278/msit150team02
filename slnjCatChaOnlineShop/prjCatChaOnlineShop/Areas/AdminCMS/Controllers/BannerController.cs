using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Experimental.ProjectCache;
using Microsoft.EntityFrameworkCore;
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
    
    //載入資料
    public IActionResult LoadDataTable()
        {
            var data = _cachaContext.GameShopBanner.Select(x => new
            {
                Banner=x.Banner,
                BannerId=x.BannerId,
                PublishDate=x.PublishDate,
                Link=x.Link,
                Display=x.Display
            }).ToList();
            return Json(new { data });

        }
        //編輯
        public IActionResult EditBanner(int? id)
        {
            try
            {
                if (id == null)
                {
                    return Json(new { success = false, message = "ID不存在" });
                }

                GameShopBanner banner = _cachaContext.GameShopBanner.FirstOrDefault(p => p.BannerId == id);

                if (banner == null)
                {
                    return Json(new { success = false, message = "Banner不存在" });
                }

                return Json(new { success = true, data = banner });
            }
            catch (Exception ex)
            {
                return BadRequest($"發生錯誤：{ex.Message}");
            }
        }


        //上傳圖片
        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile image)
        {
            try
            {
                if (image == null || image.Length == 0)
                {
                    return BadRequest("未選擇圖片");
                }

                string imageUrl = await _imageService.UploadImageAsync(image);

                return Ok(new { imageUrl = $"{imageUrl}" });
            }
            catch (Exception ex)
            {
                return BadRequest($"圖片上傳錯誤：{ex.Message}");
            }
        }



        //儲存編輯
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> EditBanner([FromForm] CBannerWrap cBanner)
        {
            try
            {
                var image = cBanner.Image;
                GameShopBanner editBanner = _cachaContext.GameShopBanner
                    .FirstOrDefault(b => b.BannerId == cBanner.BannerId);

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

                if (editBanner != null)
                {
                    if (imageURL != null)
                        editBanner.Link = imageURL;
                    if (cBanner.BannerId != null)
                        editBanner.BannerId = cBanner.BannerId;
                    if (cBanner.Banner != null)
                        editBanner.Banner = cBanner.Banner;
                    if (cBanner.PublishDate != null)
                        editBanner.PublishDate = cBanner.PublishDate;

                    _cachaContext.Update(editBanner);
                    _cachaContext.SaveChanges();
                    return Json(new { success = true, message = "Item updated successfully" });
                }
                return Json(new { success = false, message = "Item not found" });
            }
            catch (Exception ex)
            {
                return BadRequest($"發生錯誤：{ex.Message}");
            }
        }


        //新增
        [HttpPost]
        public async Task<IActionResult> CreateBanner([FromForm] CBannerWrap cBanner)
        {
            var image = cBanner.Image;

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

            var NewBanner = new GameShopBanner
            {
                BannerId = cBanner.BannerId,
                Banner = cBanner.Banner,
                PublishDate = cBanner.PublishDate,
                Display = cBanner.Display,
                Link = imageURL
            };

            try
            {
                _cachaContext.GameShopBanner.Add(NewBanner);
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
            try
            {
                if (id != null)
                {
                    GameShopBanner cBanner = _cachaContext.GameShopBanner.FirstOrDefault(p => p.BannerId == id);
                    if (cBanner != null)
                    {
                        _cachaContext.GameShopBanner.Remove(cBanner);
                        _cachaContext.SaveChanges();
                        return Json(new { success = true });
                    }
                }
                return Json(new { success = false });
            }
            catch (Exception ex)
            {
                return BadRequest($"發生錯誤：{ex.Message}");
            }
        }


        //顯示、隱藏按鈕
        [HttpPost]
        public IActionResult UpdateDisplay(bool isChecked, int bannerId)
        {
            try
            {
                var banner = _cachaContext.GameShopBanner.FirstOrDefault(b => b.BannerId == bannerId);

                if (banner == null)
                {
                    return NotFound();
                }

                banner.Display = isChecked;
                _cachaContext.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest($"錯誤：{ex.Message}");
            }
        }


    }
}
