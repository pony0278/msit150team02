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
        //=============檢查是否有此帳號跟密碼
        public IActionResult CheckAdminLogin(string account, string password)
        {
            bool result = _context.ShopGameAdminData != null && _context.ShopGameAdminData.Any(c => c.AdminAccount == account && c.AdminPassword == password);

            return Json(new { IsDuplicate = result });
        }
        [HttpPost]
        public ActionResult Login(CAdminLoginVM vm)
        {
            ShopGameAdminData user = (new cachaContext()).ShopGameAdminData.FirstOrDefault(
                t => t.AdminAccount.Equals(vm.AdminAccount) && t.AdminPassword.Equals(vm.AdminPassword));
            if (user != null && user.AdminPassword.Equals(vm.AdminPassword))
            {
                string Json = JsonSerializer.Serialize(user);
                HttpContext.Session.SetString(CAdminLogin.SK_LOGINED_USER, Json);
                return RedirectToAction("Member", "Member");
            }
            //登錄失敗返回登入頁
            return View();
        }
    }
}
