using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using prjCatChaOnlineShop.Models.CDictionary;

namespace prjCatChaOnlineShop.Controllers.Home
{

    public class MemberLogoutController : ControllerBase
    {
        [HttpPost]
        public IActionResult Logout()
        {
            try
            {
                HttpContext.Session.Remove(CDictionary.SK_LOINGED_USER); // 清除特定的 Session 變數
                HttpContext.Session.Remove("UserName");
                return RedirectToAction("Index", "Index"); // 導向登入頁面或其他適當的地方
            }
            catch (Exception ex)
            {
                // 處理異常情況
                return StatusCode(500, "發生錯誤：" + ex.Message);
            }

        }
    }
}
