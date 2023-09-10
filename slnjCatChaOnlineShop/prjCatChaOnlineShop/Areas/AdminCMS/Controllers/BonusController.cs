using ChatGPT.Net.DTO.ChatGPTUnofficial;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Experimental.ProjectCache;
using Microsoft.EntityFrameworkCore;
using NETCore.MailKit.Core;
using prjCatChaOnlineShop.Areas.AdminCMS.Models;
using prjCatChaOnlineShop.Models;
using prjCatChaOnlineShop.Services.Function;
using System.Net.Mail;

namespace prjCatChaOnlineShop.Controllers.CMS
{
    [Area("AdminCMS")]
    public class BonusController : Controller
    {
        private readonly cachaContext _cachaContext;
        public BonusController(cachaContext cachaContext)
        {
            _cachaContext = cachaContext;
        }

        public IActionResult Bonus()
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
            var data = _cachaContext.ShopCouponTotal.Select(x => new
            {
                CouponId = x.CouponId,
                CouponName = x.CouponName,
                CouponContent = x.CouponContent,
                ExpiryDate = x.ExpiryDate,
                SpecialOffer = x.SpecialOffer,
                Usable = x.Usable == null ? "未設定" : (x.Usable == true ? "是" : "否")
            }).ToList();
            return Json(new { data });
        }

        //編輯
        [HttpGet]
        public IActionResult EditCoupon(int? id)
        {
            try
            {
                if (id == null)
                {
                    return Json(new { success = false, message = "ID不存在" });
                }
                ShopCouponTotal coupon = _cachaContext.ShopCouponTotal
                                                        .FirstOrDefault(p => p.CouponId == id);

                if (coupon == null)
                {
                    return Json(new { success = false, message = "優惠券不存在" });
                }
                return Json(new { success = true, data = coupon });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "出現了一個錯誤：" + ex.Message });
            }
        }


        //儲存編輯
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> EditCoupon([FromForm] CCouponWrap cCoupon)
        {
            try
            {
                var editCoupon = _cachaContext.ShopCouponTotal
                    .FirstOrDefault(c => c.CouponId == cCoupon.CouponId);

                if (editCoupon != null)
                {
                    if (cCoupon.CouponId != null)
                        editCoupon.CouponId = cCoupon.CouponId;
                    if (cCoupon.CouponName != null)
                        editCoupon.CouponName = cCoupon.CouponName;
                    if (cCoupon.CouponContent != null)
                        editCoupon.CouponContent = cCoupon.CouponContent;
                    if (cCoupon.ExpiryDate != null)
                        editCoupon.ExpiryDate = cCoupon.ExpiryDate;
                    if (cCoupon.SpecialOffer != null)
                        editCoupon.SpecialOffer = cCoupon.SpecialOffer;
                    if (cCoupon.Usable != null)
                        editCoupon.Usable = cCoupon.Usable;

                    _cachaContext.Update(editCoupon);
                    _cachaContext.SaveChanges();
                    return Json(new { success = true, message = "優惠券已成功更新" });
                }
                return Json(new { success = false, message = "優惠券未找到" });
            }
            catch (Exception ex)
            {
                // 在這裡處理異常，可以記錄錯誤、返回自定義錯誤消息，或者進行其他適當的處理
                return Json(new { success = false, message = "出現了一個錯誤：" + ex.Message });
            }
        }

        //新增
        [HttpPost]
        public async Task<IActionResult> CreateCoupon([FromForm] CCouponWrap cCoupon)
        {
            try
            {
                var newCoupon = new ShopCouponTotal
                {
                    CouponId = cCoupon.CouponId,
                    CouponName = cCoupon.CouponName,
                    CouponContent = cCoupon.CouponContent,
                    ExpiryDate = cCoupon.ExpiryDate,
                    SpecialOffer = cCoupon.SpecialOffer,
                    Usable = cCoupon.Usable
                };
                _cachaContext.ShopCouponTotal.Add(newCoupon);
                await _cachaContext.SaveChangesAsync();
                return Json(new { success = true, message = "內容已保存！" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "出現了一個錯誤：" + ex.Message });
            }
        }


        //刪除
        [HttpPost]
        public IActionResult Delete(int? id)
        {
            try
            {
                if (id != null)
                {
                    ShopCouponTotal cCoupon = _cachaContext.ShopCouponTotal.FirstOrDefault(p => p.CouponId == id);
                    if (cCoupon != null)
                    {
                        _cachaContext.ShopCouponTotal.Remove(cCoupon);
                        _cachaContext.SaveChanges();
                        return Json(new { success = true });
                    }
                }
                return Json(new { success = false });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }


        //發送給所有會員
        [HttpPost]
        public IActionResult SendCouponToMembers(int? id)
        {
            try
            {
                ShopCouponTotal coupon = _cachaContext.ShopCouponTotal.FirstOrDefault(c => c.CouponId == id);

                if (coupon == null)
                {
                    return NotFound();
                }

                //獲取所有會員
                List<ShopMemberInfo> members = _cachaContext.ShopMemberInfo.ToList();

                foreach (var member in members)
                {
                    // 創建會員優惠券資料
                    ShopMemberCouponData memberCouponData = new ShopMemberCouponData
                    {
                        MemberId = member.MemberId,
                        CouponId = coupon.CouponId,
                        CouponStatusId = false //設置為可使用
                    };

                    _cachaContext.ShopMemberCouponData.Add(memberCouponData);
                }

                _cachaContext.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "出現了一個錯誤：" + ex.Message });
            }
        }




        //發送給單一會員
        [HttpPost]
        public IActionResult SendCouponToMembersByID(int? id, int? memberId)
        {
            try
            {

                var member = _cachaContext.ShopMemberInfo.FirstOrDefault(m => m.MemberId == memberId);

                if (member == null)
                {
                    return Json(new { success = false, message = "會員不存在" });
                }


                ShopCouponTotal coupon = _cachaContext.ShopCouponTotal.FirstOrDefault(c => c.CouponId == id);

                if (coupon == null)
                {
                    return Json(new { success = false, message = "優惠券不存在" });
                }

                // 創建會員優惠券資料
                ShopMemberCouponData memberCouponData = new ShopMemberCouponData
                {
                    MemberId = memberId.Value,
                    CouponId = coupon.CouponId,
                    CouponStatusId = false //設置為可使用
                };

                _cachaContext.ShopMemberCouponData.Add(memberCouponData);
                _cachaContext.SaveChanges();

                return Json(new { success = true });
            }

            catch (Exception ex)
            {
                return Json(new { success = false, message = "出現了一個錯誤：" + ex.Message });
            }
        }

        [HttpPost]
        public IActionResult SendCouponExpirationEmail(int? id)
        {
            var coupon = _cachaContext.ShopCouponTotal
                .Where(c => c.CouponId == id && c.Usable == true)
                .FirstOrDefault();

            if (coupon != null)
            {
                var members = _cachaContext.ShopMemberCouponData
                    .Where(m => m.CouponId == coupon.CouponId && m.CouponStatusId == false)
                    .ToList();

                foreach (var member in members)
                {
                    // 獲取會員的Email
                    var memberInfo = _cachaContext.ShopMemberInfo
                        .Where(mi => mi.MemberId == member.MemberId)
                        .FirstOrDefault();
                    var newsletterTemplete = _cachaContext.NewsletterTemplate.FirstOrDefault();
                    if (memberInfo != null)
                    {
                        // 創建郵件內容
                        string subject = "CatCha 優惠券到期提醒";
                        string image = newsletterTemplete.HeaderImage;
                        string body = $@"
                            <html>
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>優惠券到期提醒</title>
    <style>
        body {{
            font-family: Dosis, sans-serif;
            font-weight: bold;
            background-color: #ffffff;
            margin: 0;
            padding: 0;
        }}

        .container {{
            max-width: 600px;
            margin: 0 auto;
            background-color: #edd2b4;
            padding: 20px;
            border-radius: 10px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
        }}

        h1 {{
            color: black;
        }}

        p {{
            color: black;
            font-size: 18px;
        }}

        .coupon {{
            background-color: #DE9E4F;
            padding: 10px;
            border-radius: 5px;
            margin-top: 20px;
        }}

        .expiration {{
            color: #ff0000;
            font-weight: bold;
        }}

        .footer {{
            margin-top: 20px;
            text-align: center;
            color: #777;
        }}

        /* 新增的樣式用於顯示圖片 */
        .image-container {{
            text-align: center;
            margin-top: 20px;
        }}

        /* 調整圖片大小 */
        .coupon-image {{
            max-width: 100%;
            height: auto;
        }}

        .separator {{
            border-top: 1px solid #DE9E4F;
            margin-top: 20px;
        }}

    </style>
</head>

<body>
    <div class=""container"">
        <div class=""image-container"">
            <img src={image} alt=""Coupon Image"" class=""coupon-image"">
        </div>
        <h1>優惠券到期提醒</h1>
        <div class=""separator""></div>
        <p style='font-size: 20px;'>親愛的會員 {memberInfo.Name} 您好，</p>
        <p>這是一封提醒您的優惠券即將到期的郵件，請在優惠券到期前使用，以免失效。</p>
        <p>以下是有關您的優惠券的詳細訊息：</p>
        <div class=""coupon"">
            <p>優惠券名稱：<strong>{coupon.CouponName}</strong></p>
            <p>到期日：<strong>{coupon.ExpiryDate.Value.ToString("yyyy-MM-dd")}</strong></p>
        </div>
        <p>提醒您於到期日前使用完畢唷！</p>
        <div class=""footer"">
            <p>CatCha貓抓抓感謝您的支持，祝您購物愉快</p>
        </div>
    </div>
</body>
</html>
";
                        

                        // 創建郵件寄件者和收件者
                        MailAddress from = new MailAddress("catcha20232023@gmail.com", "catcha貓抓抓");
                        MailAddress to = new MailAddress(memberInfo.Email);

                        // 創建郵件物件並發送
                        using (SmtpClient smtp = new SmtpClient("smtp.gmail.com"))
                        {
                            smtp.Port = 587;
                            smtp.Credentials = new System.Net.NetworkCredential("catcha20232023@gmail.com", "qwgbrrcdglyquavh");
                            smtp.EnableSsl = true;

                            using (MailMessage message = new MailMessage(from, to))
                            {
                                message.Subject = subject;
                                message.Body = body;
                                message.IsBodyHtml = true;

                                try
                                {
                                    smtp.Send(message);
                                    Console.WriteLine("郵件發送成功！");
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("郵件發送失敗：" + ex.Message);
                                }
                            }
                        }
                    }
                }

                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}



