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
    public class GetUserIdController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public GetUserIdController(IHttpContextAccessor httpContextAccessor)
        {

            _httpContextAccessor = httpContextAccessor;
        }

        #region 取得名字
        public IActionResult GetUserId()
        {
            var memberInfoJson = _httpContextAccessor.HttpContext?.Session.GetString(CDictionary.SK_LOINGED_USER);
            if (memberInfoJson != null)
            {
                var memberInfo = JsonSerializer.Deserialize<ShopMemberInfo>(memberInfoJson);
                return new JsonResult(memberInfo.MemberId);
            }
            return Content("取得ID失敗");
        }

        #endregion

    }
}
