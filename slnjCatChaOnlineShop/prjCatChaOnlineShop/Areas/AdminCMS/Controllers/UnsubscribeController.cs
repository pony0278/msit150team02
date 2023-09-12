using Microsoft.AspNetCore.Mvc;
using prjCatChaOnlineShop.Models;
using prjCatChaOnlineShop.Services.Function;
using System.Security.Cryptography;
using System.Text;

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
        public IActionResult Unsubscribe(string token)
        {
            try
            {
                if (!string.IsNullOrEmpty(token))
                {
                    //====================將token與並比較哈希值以查找匹配的電子郵件地址

                    // 撈出電子郵件以及訂閱狀態是否為 "是" 的email，
                    string[] emailAddresses = _context.ShopMemberInfo.Where(member => member.Subscribe == true).Select(member => member.Email).ToArray();

                    foreach (var emailAddress in emailAddresses)
                    {
                        string generatedToken = GenerateUnsubscribeToken(emailAddress);

                        if (token.Equals(generatedToken, StringComparison.OrdinalIgnoreCase))
                        {

                            var sortedMembers = _context.ShopMemberInfo.OrderBy(member => member.MemberId); // 根據需要的屬性進行排序
                            var member = sortedMembers.LastOrDefault(m => m.Email == emailAddress);

                            // token匹配，找到相應的電子郵件地址
                            //var member = _context.ShopMemberInfo.LastOrDefault(m => m.Email == emailAddress);

                            if (member != null)
                            {
                                // 更新會員訂閱欄位為 false
                                member.Subscribe = false;
                                _context.SaveChanges();

                                // 返回取消訂閱成功的JSON結果
                                return Json(new { success = true, message = "取消訂閱成功" });
                            }
                        }
                    }

                    // 如果未找到相應的電子郵件地址，返回取消訂閱失敗的JSON結果或錯誤消息
                    return Json(new { success = false, message = "未找到符合的會員" });
                }
                else
                {
                    // 如果 token 為空，返回相應的錯誤消息
                    return Json(new { success = false, message = "token參數為空" });
                }
            }
            catch (Exception ex)
            {
                // 在發生異常時捕獲並處理異常情況
                // 這裡可以記錄日誌或者返回一個錯誤消息
                return Json(new { success = false, message = "取消訂閱時發生錯誤：" + ex.Message });
            }
        }

        // 生成取消訂閱 token（與前面的代碼一致）
        static string GenerateUnsubscribeToken(string emailAddress)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(emailAddress));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }
}
