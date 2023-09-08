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
using System.Net.Mime;
using System.Drawing;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Web;
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
                    memberData.Subscribe = editMember.Subscribe;
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
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> SentNewsLetter([FromForm] CSentNewsLetter cAnnounce, [FromForm] string[] recipientEmails)
        {
            var image = cAnnounce.ContentImage;

            if (image == null || image.Length == 0)
            {
                return BadRequest("No image provided.");
            }

            if (recipientEmails == null || recipientEmails.Length == 0)
            {
                return BadRequest("No recipient email provided.");
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
            try
            {
                foreach (string recipientEmail in recipientEmails)
                {
                    var newAnnounce = new Newsletter
                    {
                        TemplateId = cAnnounce.TemplateId,
                        Subject = cAnnounce.Subject,
                        ContentImage = imageUrl,
                        ImageUrl = cAnnounce.ImageUrl,
                        SendDate = DateTime.Now,
                    };

                    _context.Newsletter.Add(newAnnounce);
                    await _context.SaveChangesAsync();

                    // 成功保存數據後，調用發送郵件的 API
                    sendTest(newAnnounce.NewsletterId, recipientEmail); // 傳遞新插入的Newsletter的ID和收件人郵箱
                }
            }
            catch
            {
                return BadRequest("Error saving the announcement.");
            }
            return Json(new { success = true, message = "Content saved!" });
        }
        public List<string> GetImageUrlsFromDatabase()
        {
            List<string> imageUrls = _context.Newsletter.Select(template => template.ContentImage).ToList();
            return imageUrls;
        }
        [HttpPost]
        public IActionResult sendTest(int newsletterId, string recipientEmailAddress)
        {
            string senderEmail = "rong502njc@gmail.com"; //寄件人mail
            string senderPassword = "wdyhdmvdwniptybf"; //應用程式密碼： 如果您的 Gmail 帳號啟用了雙重驗證，您需要使用應用程式密碼而不是您的 Gmail 登錄密碼。您可以在 Google 帳號的安全性設置中建立一個應用程式密碼，然後將它用作您的密碼。

            // 收件人的mail
            string recipientEmail = recipientEmailAddress;

            var newsletter = _context.Newsletter.FirstOrDefault();

            var insertedNewsletter = _context.Newsletter.FirstOrDefault(n => n.NewsletterId == newsletterId);

            var newsletterTemplete = _context.NewsletterTemplate.FirstOrDefault();

            if (insertedNewsletter != null)
            {
                string mailSubject = insertedNewsletter.Subject;
                string imgHeader = newsletterTemplete.HeaderImage;
                string imageSrc = insertedNewsletter.ContentImage;
                string imageLink = insertedNewsletter.ImageUrl;
                string imgFooter = newsletterTemplete.FooterImage;

                // 設置發件人的名稱和郵箱地址
                MailAddress fromAddress = new MailAddress(senderEmail, "catCha 貓抓抓"); // 後面的參數是想要顯示的姓名

                // 設置SMTP客户端信息
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(senderEmail, senderPassword),
                    EnableSsl = true,
                };

                // 將多個收件人拆分為單獨的電子郵件地址
                string[] emailAddresses = recipientEmail.Split(',');

                foreach (string emailAddress in emailAddresses)
                {
                    // 為每個收件人生成取消訂閱連結
                    string unsubscribeUrl = $"https://localhost:7218/AdminCMS/Unsubscribe/UnsubscribeView?email={emailAddress.Trim()}";

                    // 創建郵件並設置其内容
                    MailMessage mailMessage = new MailMessage(senderEmail, emailAddress)
                    {
                        From = fromAddress,
                        Subject = mailSubject,
                        Body = GenerateEmailBody(unsubscribeUrl),
                        IsBodyHtml = true
                    };

                    // 添加CC收件人
                    mailMessage.CC.Add(new MailAddress("rong502njc@gmail.com", "貓抓抓管理員")); // CC收件人mail和姓名

                    // 發送郵件...
                    try
                    {
                        smtpClient.Send(mailMessage);
                        Console.WriteLine("郵件發送成功！");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("郵件發送失敗：" + ex.Message);
                    }
                }

                // 生成郵件的 HTML 内容
                string GenerateEmailBody(string unsubscribeUrl)
                {
                    string htmlBody = $@"
    <html>
    <head>
        <link rel='stylesheet' type='text/css' href='/css/newsletter.css'>
    </head>
    <body>
        <table align='center' cellspacing='0' cellpadding='0' width='80%'>
            <tr>
                <td style='padding: 0 2rem;'>
                </td>
                <td style='text-align: center;'>
                    <p style='font-size: 14px;font-weight: 600;color: #595a5c;text-align: center;'>【此信件為系統自動發送，請勿直接回覆】</p>
                    <img src='{imgHeader}' alt='Image' style='max-width: 100%;' />

                <h1 style='margin-bottom: 28px;color: #000;'>mao&kou | 實木貓爬架</h1>
                <h2 style='background-color: #dc143c;display: inline;padding: 10px 15px 10px 15px;border-radius: 34px;color: #FFF;'>現正特惠中</h2>
                <h2 style='margin-top: 30px;color: #000;'>寵愛價<span style='font-size: 45px;color: crimson;'>79</span>折起</h2>
                
                    <div>
                        <a href='{imageLink}'><img src='{imageSrc}' alt='Image' style='width: 500px;' /></a>
                    </div>
                        <a href='{imageLink}' style='display: block;background-color: #b95756;border-radius: 0px;color: #ffffff;display: inline-block;font-size: 18px;line-height: 48px;text-align: center;text-decoration: none;width: 185px;font-weight: 900;border: 4px solid #b95756;margin-top: 30px;margin-bottom: 30px;'>前往選購</a>
                    <img src = '{imgFooter}' alt ='Image' style='max-width: 100%;' />
                    <div style='background-color: #f0eff0;padding: 30px; text-align: center;' >
                        <!-- 在這裡插入取消訂閱連結 -->
                        <p>隱私條款 | 服務使用規範 | <a href='{unsubscribeUrl}'>取消訂閱電子報</a></p>
                        <p>106 台北市大安區復興南路一段 390 號 2 樓 © 2023 catCha Taiwan</p>
                    </div>
                </td>
                <td style = 'padding: 0 2rem;' >
                </td>
            </tr>
        </table>
    </body>
    </html>";

                    // 將取消訂閱連結插入到 HTML 内容中
                    htmlBody = htmlBody.Replace("{UnsubscribeLink}", unsubscribeUrl);

                    return htmlBody;
                }
            }
            else
            {
                Console.WriteLine("無可用的Newsletter數據。");
            }
            return Json(new { success = true, message = "發送信件成功" });
        }
    }
}
