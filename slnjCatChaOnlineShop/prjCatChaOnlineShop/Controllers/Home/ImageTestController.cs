﻿using Microsoft.AspNetCore.Mvc;
using prjCatChaOnlineShop.Services.Function;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using prjCatChaOnlineShop.Models.CModels;
using prjCatChaOnlineShop.Models;
using prjCatChaOnlineShop.Areas.AdminCMS.Models;

namespace prjCatChaOnlineShop.Controllers.Home
{
    public class ImageTestController : Controller
    {

        private readonly ImageService _imageService;
        private readonly cachaContext _cachaContext;

        public ImageTestController(ImageService imageService , cachaContext cachaContext)
        {
            _imageService = imageService;
            _cachaContext = cachaContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile image, string AnnouncementContent)
        {
            if (image == null || image.Length == 0)
            {
                return BadRequest("No image provided.");
            }

            string imageUrl;
            try
            {
                imageUrl = await _imageService.UploadImageAsync(image);
            }
            catch
            {
                
                return BadRequest("Error uploading the image.");
            }

            if (string.IsNullOrWhiteSpace(AnnouncementContent))
            {
                return BadRequest("Announcement content cannot be empty.");
            }

            var newAnnounce = new GameShopAnnouncement
            {
                AnnouncementImageHeader = imageUrl,
                AnnouncementContent = AnnouncementContent
            };

            try
            {
                _cachaContext.GameShopAnnouncement.Add(newAnnounce);
                await _cachaContext.SaveChangesAsync();
            }
            catch
            {
                return BadRequest("Error saving the announcement.");
            }

            return Json(new { uploaded = true, url = $"{imageUrl}" });
        }

        [HttpPost]
        public async Task<IActionResult> UploadImageToMemberInfo(IFormFile image, string AnnouncementContent)
        {
            if (image == null || image.Length == 0)
            {
                return BadRequest("No image provided.");
            }

            string imageUrl;
            try
            {
                imageUrl = await _imageService.UploadImageAsync(image);
            }
            catch
            {

                return BadRequest("Error uploading the image.");
            }

            if (string.IsNullOrWhiteSpace(AnnouncementContent))
            {
                return BadRequest("Announcement content cannot be empty.");
            }

            try
            {
                if (int.TryParse(Request.Form["memberIdForMembership"], out int memberIdForMembership))
                {
                    var memberToUpdate = _cachaContext.ShopMemberInfo.FirstOrDefault(m => m.MemberId == memberIdForMembership);
                    memberToUpdate.MemberImage = imageUrl;
                    await _cachaContext.SaveChangesAsync();
                }

            }
            catch
            {
                return BadRequest("Error saving the announcement.");
            }

            return RedirectToAction("membership", "membership");
        }

        [HttpPost]
        public async Task<IActionResult> UploadImageToImageModerator(IFormFile image, string AnnouncementContent)
        {
            if (image == null || image.Length == 0)
            {
                return BadRequest("No image provided.");
            }

            string imageUrl;
            try
            {
                imageUrl = await _imageService.UploadImageAsync(image);
            }
            catch
            {

                return BadRequest("Error uploading the image.");
            }

            if (string.IsNullOrWhiteSpace(AnnouncementContent))
            {
                return BadRequest("Announcement content cannot be empty.");
            }

            return Json(new { uploaded = true, url = $"{imageUrl}" });
        }
    }
}
