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
                TotalQuantity = x.TotalQuantity,
                Usable = x.Usable == null ? "未設定" : (x.Usable == true ? "是" : "否")
            }).ToList();
            return Json(new { data });
        }

        //編輯
        [HttpGet]
        public IActionResult EditCoupon(int? id)
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

        //儲存編輯
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> EditCoupon([FromForm] CCouponWrap cCoupon)
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
                if (cCoupon.TotalQuantity != null)
                    editCoupon.TotalQuantity = cCoupon.TotalQuantity;
                if (cCoupon.Usable != null)
                    editCoupon.Usable = cCoupon.Usable;

                _cachaContext.Update(editCoupon);
                _cachaContext.SaveChanges();
                return Json(new { success = true, message = "Item updated successfully" });
            }
            return Json(new { success = false, message = "Item not found" });
        }

        //新增
        [HttpPost]
        public async Task<IActionResult> CreateCoupon([FromForm] CCouponWrap cCoupon)
        {
            var newCoupon = new ShopCouponTotal
            {
                CouponId = cCoupon.CouponId,
                CouponName = cCoupon.CouponName,
                CouponContent = cCoupon.CouponContent,
                ExpiryDate = cCoupon.ExpiryDate,
                TotalQuantity = cCoupon.TotalQuantity,
                Usable = cCoupon.Usable
            };
            _cachaContext.ShopCouponTotal.Add(newCoupon);
            await _cachaContext.SaveChangesAsync();
            return Json(new { success = true, message = "Content saved!" });
        }

        //刪除
        [HttpPost]
        public IActionResult Delete(int? id)
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

        //發送給所有會員
        [HttpPost]
        public IActionResult SendCouponToMembers(int? id)
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
                    CouponStatusId = true //設置為可使用
                };

                _cachaContext.ShopMemberCouponData.Add(memberCouponData);
            }

            _cachaContext.SaveChanges();

            return Json(new { success = true });
        }



        //發送給單一會員
        [HttpPost]
        public IActionResult SendCouponToMembersByID(int? id, int? memberId)
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
                CouponStatusId = true //設置為可使用
            };

            _cachaContext.ShopMemberCouponData.Add(memberCouponData);
            _cachaContext.SaveChanges();

            return Json(new { success = true });
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
                    .Where(m => m.CouponId == coupon.CouponId && m.CouponStatusId == true)
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
                        string subject = "CatCha 優惠券到期通知";
                        string imgHeader = newsletterTemplete.HeaderImage;
                        string imgFooter = newsletterTemplete.FooterImage;
                        string body = $@"
                            <html>
                            <body>
                                <table align='center' cellspacing='0' cellpadding='0' width='100%'>
                                    <tr>
                                        <td style='padding: 0 2rem;'>
                                        </td>
                                        <td style='text-align: center;'>
                                            <p style='font-size: 18px;font-weight: 600;color: #595a5c;text-align: center;'>【此信件為系統自動發送，請勿直接回覆】</p>
                        <img src='{imgHeader}' alt='Image' style='max-width: 100%;' />
                        <div>
                                <h2>{subject}</h2>
                                <p style='font-size: 20px'>親愛的會員{memberInfo.Name} 您好，以下是有關您的優惠券的詳細訊息：</p>
                                    <p style='font-size: 18px'>優惠券名稱：<strong>{coupon.CouponName}</strong></p>
                                    <p style='font-size: 18px'>到期日：<strong>{coupon.ExpiryDate.Value.ToString("yyyy-MM-dd")}</strong></p>
                                <p style='font-size: 20px'>提醒您於到期日前使用完畢唷，CatCha貓抓抓感謝您的支持與購買！</p>
                        </div>

                                            <button style='background-color: #b95756;border-radius: 0px;color: #ffffff;display: inline-block;font-size: 18px;line-height: 48px;text-align: center;text-decoration: none;width: 185px;font-weight: 900;border: 4px solid #b95756;margin-top:30px;margin-bottom: 30px;cursor: pointer;'>前往選購</button>
                        </a>
                                            <img src = '{imgFooter}' alt = 'Image' style = 'max-width: 100%;' />
                                            <div style='background-color: #f0eff0;padding: 30px; text-align: center;' >
                                                <p>隱私條款 | 服務使用規範 | 取消訂閱電子報 </p>
                                                <p>106 台北市大安區復興南路一段 390 號 2 樓 © 2023 catCha Taiwan</p>
                                            </div>
                                        </td>
                                        <td style = 'padding: 0 2rem;' >
                                        </td>
                                    </tr>
                                </table>
                            </body>
                            </html>";


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
            return NotFound();
        }
    }
}



