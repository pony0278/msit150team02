using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
//using prjCatChaOnlineShop.Areas.AdminCMS.Models.ViewModels;
//using prjCatChaOnlineShop.Areas.AdminCMS.Service;
using prjCatChaOnlineShop.Models;
using System.Text.Json.Serialization;
using System.Text.Json;
using prjCatChaOnlineShop.Areas.AdminCMS.Models;
using prjCatChaOnlineShop.Models.CDictionary;
using Microsoft.Build.Experimental.ProjectCache;
using prjCatChaOnlineShop.Models.CModels;
using prjCatChaOnlineShop.Services.Function;
using System.Net.Mail;
using System.Text;
using System.Net;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using static System.Net.Mime.MediaTypeNames;
//using prjCatChaOnlineShop.Areas.AdminCMS.Util;

namespace prjCatChaOnlineShop.Controllers.CMS
{
    [Area("AdminCMS")]
    public class MemberController : Controller
    {
        private readonly cachaContext _context;
        private readonly ImageService _imageService;
        public MemberController(cachaContext context, ImageService imageService)
        {
            _context = context;
            _imageService = imageService;
        }


        //private readonly CMemberService _memberService;
        //public MemberController(CMemberService memberService)
        //{
        //    _memberService = memberService;
        //}


        public IActionResult Member()
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

        public IActionResult ShowMemeberInfo()
        {
            //ShowMemeberInfo
            //var data = _memberService.ShowMemeberInfo();

            var data = _context.ShopMemberInfo;

            return Json(new { data });
        }

        public IActionResult GetMemberDetails(int id)
        {
            var member = _context.ShopMemberInfo
                      .Include(m => m.ShopCommonAddressData)
                      .Include(m => m.ShopMemberCouponData)
                      .FirstOrDefault(m => m.MemberId == id);

            if (member != null)
            {
                foreach (var couponData in member.ShopMemberCouponData)
                {
                    // 根據 couponData.CouponId 查詢其他相關數據並附加到 couponData
                    var CouponTotal = _context.ShopCouponTotal.FirstOrDefault(o => o.CouponId == couponData.CouponId);
                }

                return Json(new { data = member });
            }
            else
            {
                // 沒有找到匹配的成員
                //  do something..........
                return NotFound();
            }
        }
        //=============新增會員檢查帳號是否重複
        public IActionResult CheckDuplicateAccount(string account)
        {
            bool result = _context.ShopMemberInfo != null && _context.ShopMemberInfo.Where(c => c.MemberAccount == account).Count() >= 1;


            return Json(new { IsDuplicate = result });
        }
        [HttpPost]
        public IActionResult AddMember(ShopMemberInfo newMember)
        {
            // 後端待補-檢查信箱&帳號格式，及檢查帳號是否已存在；
            try
            {
                if (newMember != null) // 檢查 newMember 是否為空
                {
                    // 將 newMember 存入 _context
                    _context.ShopMemberInfo.Add(newMember);
                    _context.SaveChanges();

                    return Json(new { success = true, message = "會員新增成功！" });
                }
                else
                {
                    return Json(new { success = false, message = "新增的會員資訊為空。" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "會員新增失敗：" + ex.Message });
            }
        }

        public IActionResult EditMemeber(int id)
        {
            var member = _context.ShopMemberInfo
                      .Include(m => m.ShopCommonAddressData)
                      .FirstOrDefault(m => m.MemberId == id);

            if (member != null)
            {
                return Json(new { data = member });
            }
            else
            {
                // 沒有找到匹配的成員
                //  do something..........
                return NotFound();
            }
        }
        [HttpPost]
        public IActionResult UpdateMember(CMember editMember)
        {
            //Utility utility = new Utility();
            //if (IsValidFormat(memberData))
            //{
            var memberData = _context.ShopMemberInfo.FirstOrDefault(m => m.MemberId == editMember.MemberId);
            try
            {
                if (memberData != null) // 檢查 memberData 是否為空
                {
                    memberData.MemberId = editMember.MemberId;
                    memberData.MemberAccount = editMember.MemberAccount;
                    memberData.CharacterName = editMember.CharacterName;
                    memberData.LevelId = editMember.LevelId;
                    memberData.Name = editMember.Name;
                    memberData.Gender = editMember.Gender;
                    memberData.Birthday = editMember.Birthday;
                    memberData.Email = editMember.Email;
                    memberData.PhoneNumber = editMember.PhoneNumber;
                    memberData.MemberStatus = editMember.MemberStatus;
                    memberData.CatCoinQuantity = editMember.CatCoinQuantity;
                    memberData.LoyaltyPoints = editMember.LoyaltyPoints;
                    memberData.RunGameHighestScore = editMember.RunGameHighestScore;
                    // 更新其他字段...

                    // 將member存入DbContext
                    _context.SaveChanges();

                    return Json(new { success = true, message = "編輯會員成功！" });
                }
                else
                {
                    return Json(new { success = false, message = "編輯會員的資訊為空。" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "編輯會員失敗：" + ex.Message });
            }
        }

        //private bool IsValidFormat(CMember memberData)
        //{
        //    if (!string.IsNullOrWhiteSpace(memberData.Name) && !string.IsNullOrWhiteSpace(memberData.Email))
        //    {
        //        if (IsValidEmail(memberData.Email))
        //        {
        //            return true;
        //        }
        //    }

        //    return false;
        //}

        //private bool IsValidEmail(string email)
        //{
        //    return true;
        //}


        //public IActionResult DeleteMemeber()
        //{
        //    return View();
        //}
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> editorUploadImage([FromForm] CNewsLetterTemplete cAnnounce)
        {
            var image1 = cAnnounce.HeaderImage;
            var image2 = cAnnounce.FooterImage;

            if (image1 == null || image1.Length == 0)
            {
                return BadRequest("No image provided.");
            }
            if (image2 == null || image2.Length == 0)
            {
                return BadRequest("No image provided.");
            }

            string imageUrl1;
            string imageUrl2;
            try
            {
                imageUrl1 = await _imageService.UploadImageAsync(image1);
                imageUrl2 = await _imageService.UploadImageAsync(image2);
            }
            catch
            {

                return BadRequest("Error uploading the image.");
            }
            var newAnnounce = new NewsletterTemplate
            {
                HeaderImage = imageUrl1,
                FooterImage = imageUrl2
            };

            try
            {
                _context.NewsletterTemplate.Add(newAnnounce);
                await _context.SaveChangesAsync();
            }
            catch
            {
                return BadRequest("Error saving the announcement.");
            }
            return Json(new { success = true, message = "Content saved!" });
        }
        //[HttpPost]
        //[Consumes("multipart/form-data")]
        //public async Task<IActionResult> SentNewsLetter([FromForm] CSentNewsLetter cAnnounce)
        //{
        //    var image = cAnnounce.ContentImage;

        //    if (image == null || image.Length == 0)
        //    {
        //        return BadRequest("No image provided.");
        //    }

        //    string imageUrl;
        //    try
        //    {
        //        imageUrl = await _imageService.UploadImageAsync(image);
        //    }
        //    catch
        //    {

        //        return BadRequest("Error uploading the image.");
        //    }
        //    var newAnnounce = new Newsletter
        //    {
        //        TemplateId = cAnnounce.TemplateId,
        //        Subject= cAnnounce.Subject,
        //        ContentImage = imageUrl,
        //        SendDate = DateTime.Now,
        //    };

        //    try
        //    {
        //        _context.Newsletter.Add(newAnnounce);
        //        await _context.SaveChangesAsync();
        //    }
        //    catch
        //    {
        //        return BadRequest("Error saving the announcement.");
        //    }
        //    return Json(new { success = true, message = "Content saved!" });
        //}
        public IActionResult sendTest()
        {
            string Account = "rong502njc@gmail.com";
            string Password = "wdyhdmvdwniptybf"; //應用程式密碼： 如果您的 Gmail 帳號啟用了雙重驗證，您需要使用應用程式密碼而不是您的 Gmail 登錄密碼。您可以在 Google 帳號的安全性設置中建立一個應用程式密碼，然後將它用作您的密碼。

            SmtpClient client = new SmtpClient();
            client.Host = "smtp.gmail.com";  //設定Server
            client.Port = 587;  //設定Port
            client.Credentials = new NetworkCredential(Account, Password);  //設定寄件人的帳號密碼
            client.EnableSsl = true;  //是否啟用SSL驗證

            //var strFromAddress = "rong502njc@gmail.com";
            //var strToAddress = "liang930517@yahoo.com.tw";
            //var strTitle = "貓抓抓-測試發送信";

            //寄件人、收信人、信件標題、信件內容  
            //MailMessage mail = new MailMessage(strFromAddress, strToAddress, strTitle, strBody);

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(Account);
            mail.To.Add("rong502njc@gmail.com");
            mail.Subject = "測試信";
            mail.SubjectEncoding = Encoding.UTF8;
            mail.IsBodyHtml = true;
            mail.Body = "第一行<br> 第二行<br>第三行<br>";
            mail.BodyEncoding = Encoding.UTF8;

            try
            {
                client.Send(mail);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                mail.Dispose();
                client.Dispose();
            }

            //string SmtpServer = "smtp.gmail.com";
            //int SmtpPort = 587;
            //MailMessage mms = new MailMessage();
            //mms.From = new MailAddress(GoogleID);
            //mms.Subject = "信件主題";
            //mms.Body = "信件內容";
            //mms.IsBodyHtml = true;
            //mms.SubjectEncoding = Encoding.UTF8;
            //mms.To.Add(new MailAddress(ReceiveMail));
            //using (SmtpClient client = new SmtpClient(SmtpServer, SmtpPort))
            //{
            //    client.EnableSsl = true;
            //    client.Credentials = new NetworkCredential(GoogleID, TempPwd);//寄信帳密 
            //    client.Send(mms); //寄出信件
            //}
            return Json(new { success = true, message = "發送信件成功" });
        }
    }
}
