using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjCatChaOnlineShop.Models.CDictionary;
using prjCatChaOnlineShop.Models;
using prjCatChaOnlineShop.Services.Function;
using System.Text.Json;
using System.Reflection;

namespace prjCatChaOnlineShop.Controllers.Home
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetUserNameController : Controller

    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public GetUserNameController(IHttpContextAccessor httpContextAccessor)
        {

            _httpContextAccessor = httpContextAccessor;
        }

        #region 取得名字
        public IActionResult GetUserName()
        {
            var memberInfoJson = _httpContextAccessor.HttpContext?.Session.GetString(CDictionary.SK_LOINGED_USER);
            if (memberInfoJson != null)
            {
                var memberInfo = JsonSerializer.Deserialize<ShopMemberInfo>(memberInfoJson);
                return Content($"Hello, {memberInfo.Name}");
            }
            return Content("登入/註冊");
        }

        #endregion

    }
}
