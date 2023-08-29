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
            HttpContext.Session.Remove(CDictionary.SK_LOINGED_USER); // 清除特定的 Session 變數
            HttpContext.Session.Remove("UserName");
            return RedirectToAction("Index", "Index"); // 導向登入頁面或其他適當的地方
        }
    }
}
