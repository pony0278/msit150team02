using prjCatChaOnlineShop.Models;
using prjCatChaOnlineShop.Models.CModels;
using prjCatChaOnlineShop.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using prjCatChaOnlineShop.Services.Function;
using prjCatChaOnlineShop.Areas.AdminCMS.Models.ViewModels;
using prjCatChaOnlineShop.Areas.AdminCMS.Models;

namespace prjCatChaOnlineShop.Areas.AdminCMS.Controllers
{
    [Area("AdminCMS")]
    public class BanUserController : Controller
    {
        private readonly cachaContext _cachaContext;

        public BanUserController(cachaContext cachaContext)
        {
            _cachaContext = cachaContext;
        }
        public IActionResult Index()
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
        [HttpGet]
        public IActionResult GetMember()
        {
            var rawData = _cachaContext.ShopMemberInfo.ToList();
            var data = rawData.Select(x => new
            {
                MemberId = x.MemberId,
                MemberAccount = x.MemberAccount,
                Name = x.Name,
                isBanned = x.IsBanned ,
                BannedTime = x.BannedTime == null ? "未禁言" : x.BannedTime.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                unBannedTime = x.UnBannedTime == null ? "未禁言" : x.UnBannedTime.Value.ToString("yyyy-MM-dd HH:mm:ss"),
            }).ToList();
            return Json(new { data });
        }
        [HttpPost]
        public IActionResult BannedUser([FromBody] CMember member)
        {
            ShopMemberInfo memberInfo = _cachaContext.ShopMemberInfo.FirstOrDefault(x=>x.MemberId == member.MemberId);
            if (memberInfo != null)
            {
                memberInfo.MemberId = member.MemberId;
                memberInfo.IsBanned = member.isBanned;
                memberInfo.BannedTime = DateTime.Now;
                memberInfo.UnBannedTime = member.unBannedTime;

                _cachaContext.Update(memberInfo);
                _cachaContext.SaveChanges();

                return Json(new { success = true, message = "成功禁言" });
            }
            return Json(new { success = false, message = "未找到成員" });
        }
    }
}
