﻿using Microsoft.AspNetCore.Mvc;
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

                    if (memberInfo != null)
                    {
                        // 創建郵件內容
                        string subject = "優惠券到期通知";
                        string body = $"優惠券名稱：{coupon.CouponName}\n到期日：{coupon.ExpiryDate}";


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
                                message.IsBodyHtml = false;

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



