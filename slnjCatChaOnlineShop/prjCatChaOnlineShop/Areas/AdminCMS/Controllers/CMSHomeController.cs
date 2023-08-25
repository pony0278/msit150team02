using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjCatChaOnlineShop.Areas.AdminCMS.Models;
using prjCatChaOnlineShop.Areas.AdminCMS.Models.ViewModels;
using prjCatChaOnlineShop.Models;
using prjCatChaOnlineShop.Models.CDictionary;
using System.Text.Json;

namespace prjCatChaOnlineShop.Areas.AdminCMS.Controllers
{
    [Area("AdminCMS")]
    public class CMSHomeController : Controller
    {
        private readonly cachaContext _context;
        public CMSHomeController(cachaContext context)
        {
            _context = context;
        }
        public IActionResult Login()
        {
            //判斷是否有登入
            if (HttpContext.Session.Keys.Contains(CAdminLogin.SK_LOGINED_USER))
            {
                return RedirectToAction("Member", "Member");
            }
            return View();
        }
        [HttpPost]
        public IActionResult Login(CAdminLoginVM vm)
        {
            ShopGameAdminData user = (new cachaContext()).ShopGameAdminData.FirstOrDefault(
                t => t.AdminAccount.Equals(vm.AdminAccount) && t.AdminPassword.Equals(vm.AdminPassword));
            if (user != null && user.AdminPassword.Equals(vm.AdminPassword))
            {
                string Json = JsonSerializer.Serialize(user);
                HttpContext.Session.SetString(CAdminLogin.SK_LOGINED_USER, Json);

                // 將管理員姓名用session存起來
                HttpContext.Session.SetString("AdminName", user.AdminUsername);

                return RedirectToAction("Member", "Member");
            }
            else
            {
                ViewBag.ErrorMessage = "帳號或密碼錯誤。請重新輸入。";
                //登錄失敗返回登入頁
                return View();
            }
        }

        //====================登出功能
        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove(CAdminLogin.SK_LOGINED_USER);
            return RedirectToAction("Login", "CMSHome");
        }
    }
}
