using Microsoft.AspNetCore.Mvc;
using prjCatChaOnlineShop.Areas.AdminCMS.Models;
using prjCatChaOnlineShop.Models;
using System.Security.Policy;

namespace prjCatChaOnlineShop.Areas.AdminCMS.Controllers

{
    [Area("AdminCMS")]
    public class AdminManageController : Controller
    {
        private readonly cachaContext _context;
        public AdminManageController(cachaContext context)
        {
            _context = context;
        }
        public IActionResult AdminManage()
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

        public IActionResult ShowAdminManage()
        {
            var data = _context.ShopGameAdminData;
            return Json(new { data });
        }
        [HttpPost]
        public IActionResult AddAdminManage(ShopGameAdminData newAdmin)
        {
            try
            {
                if (newAdmin != null) // 檢查 newAdmin 是否為空
                {
                    // 將 newAdmin 存入 _context
                    _context.ShopGameAdminData.Add(newAdmin);
                    _context.SaveChanges();

                    return Json(new { success = true, message = "管理員新增成功！" });
                }
                else
                {
                    return Json(new { success = false, message = "新增的管理員資訊為空。" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "管理員新增失敗：" + ex.Message });
            }
        }
        //=============新增管理員檢查帳號是否重複
        public IActionResult CheckDuplicateAccount(string account, string name)
        {
            bool accountExists = _context.ShopGameAdminData.Any(c => c.AdminAccount == account);
            bool nameExists = _context.ShopGameAdminData.Any(c => c.AdminUsername == name);

            return Json(new { IsDuplicateAccount = accountExists, IsDuplicateName = nameExists });
        }
    }
}
