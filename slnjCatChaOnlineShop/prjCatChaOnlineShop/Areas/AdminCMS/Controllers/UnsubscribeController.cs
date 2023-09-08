using Microsoft.AspNetCore.Mvc;
using prjCatChaOnlineShop.Models;
using prjCatChaOnlineShop.Services.Function;

namespace prjCatChaOnlineShop.Areas.AdminCMS.Controllers
{
    [Area("AdminCMS")]
    public class UnsubscribeController : Controller
    {
        private readonly cachaContext _context;
        public UnsubscribeController(cachaContext context)
        {
            _context = context;
        }
        public IActionResult UnsubscribeView()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Unsubscribe(string emails)
        {
            try
            {
                // 驗證郵箱參數，並將字符串分割成郵件地址的列表
                if (!string.IsNullOrEmpty(emails))
                {
                    List<string> emailList = emails.Split(',').Select(email => email.Trim()).ToList();

                    foreach (var email in emailList)
                    {
                        // 驗證郵箱參數，並從數據庫中查找會員
                        var member = _context.ShopMemberInfo.FirstOrDefault(m => m.Email == email);

                        if (member != null)
                        {
                            // 更新會員訂閱字段為 false
                            member.Subscribe = false;
                            _context.SaveChanges();

                            // 返回取消訂閱成功的JSON結果
                            return Json(new { success = true, message = "取消訂閱成功！" });
                        }
                    }

                    // 如果未找到相關會員，返回取消訂閱失敗的JSON結果或錯誤消息
                    return Json(new { success = false, message = "未找到符合的會員！" });
                }
                else
                {
                    // 如果 emails 為空，返回相應的錯誤消息
                    return Json(new { success = false, message = "email參數為空！" });
                }
            }
            catch (Exception ex)
            {
                // 在發生異常時捕獲並處理異常情況
                // 這裡您可以記錄日誌或者返回一個錯誤消息
                return Json(new { success = false, message = "取消訂閱時發生錯誤：" + ex.Message });
            }
        }
    }
}
