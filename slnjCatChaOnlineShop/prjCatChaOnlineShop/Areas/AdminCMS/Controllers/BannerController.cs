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
                Link=x.Link
            }).ToList();
            return Json(new { data });

        }
        //編輯
        public IActionResult EditBanner(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "ID不存在" });
            }
            GameShopBanner banner = _cachaContext.GameShopBanner
                                                                    .FirstOrDefault(p => p.BannerId == id);

            if (banner == null)
            {
                return Json(new { success = false, message = "Banner不存在" });
            }
            return Json(new { success = true, data = banner });

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


        //儲存編輯
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> EditBanner([FromForm] CBannerWrap cBanner)
        {
            var image = cBanner.Image;
            var editBanner = _cachaContext.GameShopBanner
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
            var insertImgList = _cachaContext.GameShopBanner
                                .Where(x => x.BannerId == cBanner.BannerId)
                                .ToList();

            List<string> imageUrls = new List<string>();
            if (cBanner.BannerImage != null && cBanner.BannerImage.Count > 0)
            {
                for (int i = 0; i < cBanner.BannerImage.Count; i++)
                {
                    try
                    {
                        var uploadedImageUrl = await _imageService.UploadImageAsync(cBanner.BannerImage[i]);
                        imageUrls.Add(uploadedImageUrl);

                        if (i < insertImgList.Count)
                        {
                            insertImgList[i].Link = uploadedImageUrl;
                        }
                        else
                        {
                            var newImageItem = new GameShopBanner
                            {
                                BannerId = cBanner.BannerId,
                                Link = uploadedImageUrl
                            };
                            _cachaContext.GameShopBanner.Add(newImageItem);
                            _cachaContext.SaveChanges();
                        }
                    }
                    catch
                    {
                        return BadRequest("圖片上傳錯誤.");
                    }
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

        //新增
        [HttpPost]
        public async Task<IActionResult> CreateBanner([FromForm] CBannerWrap cBanner)
        {
            var NewBanner = new GameShopBanner
            {
                BannerId = cBanner.BannerId,
                Banner = cBanner.Banner,
                PublishDate = cBanner.PublishDate
            };
            _cachaContext.GameShopBanner.Add(NewBanner);
            await _cachaContext.SaveChangesAsync();

            List<string> imageUrls = new List<string>();
            if (cBanner.BannerImage != null && cBanner.BannerImage.Count > 0)
            {
                for (var i = 0; i < cBanner.BannerImage.Count; i++)
                {
                    try
                    {
                        var uploadedImageUrl = await _imageService.UploadImageAsync(cBanner.BannerImage[i]);
                        var newImageItem = new GameShopBanner
                        {
                            BannerId = NewBanner.BannerId,
                            Link = uploadedImageUrl
                        };
                        _cachaContext.GameShopBanner.Add(newImageItem);
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



    }
}
