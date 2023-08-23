using Microsoft.AspNetCore.Mvc;
using prjCatChaOnlineShop.Models.CDictionary;
using prjCatChaOnlineShop.Models.ViewModels;
using prjCatChaOnlineShop.Models;
using System.Text.Json;

namespace prjCatChaOnlineShop.Controllers.Home
{
    public class MemberLoginController : Controller
    {
        //將 _context 注入控制器，可以在控制器的操作方法中使用 _context 來執行資料庫查詢和操作
        private readonly cachaContext _context;
        private readonly IConfiguration _configuration;
        public MemberLoginController(cachaContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(CLoginModel vm)
        {
            //確定有抓到全部會員的資料，若資料表信箱或密碼其中有空值就會跳例外錯誤
            ShopMemberInfo user = (new cachaContext()).ShopMemberInfo.FirstOrDefault(
                t => t.Email.Equals(vm.txtEmail) && t.Password.Equals(vm.txtPassword));
            if (user != null && user.Password.Equals(vm.txtPassword))
            {
                string Json = JsonSerializer.Serialize(user);
                HttpContext.Session.SetString(CDictionary.SK_LOINGED_USER, Json);
                return RedirectToAction("Index", "Index");
            }
            return View();
        }

        public IActionResult ForgetPassword()
        {
            return View();
        }

        public IActionResult RegisterMember() 
        {
            return View();
        }
        [HttpPost]
        public IActionResult RegisterMember(ShopMemberInfo registerModel)
        {

            _context.ShopMemberInfo.Add(registerModel);
            _context.SaveChanges();

            return RedirectToAction("Login");

        }
        //驗證信箱是否存在
        [HttpPost]
        public JsonResult CheckEmailExist(string email)
        {
            bool emailExist = _context.ShopMemberInfo.Any(x => x.Email == email);
            return Json(emailExist);
        }
    }
}
