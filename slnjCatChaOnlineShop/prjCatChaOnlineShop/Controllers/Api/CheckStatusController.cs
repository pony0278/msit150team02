using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using prjCatChaOnlineShop.Models;
using prjCatChaOnlineShop.Models.CDictionary;
using prjCatChaOnlineShop.Models.CModels;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using OpenAI_API.Moderation;

namespace prjCatChaOnlineShop.Controllers.Api
{

    //[Route("api/Api/[controller]")]
    //[ApiController]


    public class CheckStatusController : ControllerBase
    {

        private readonly cachaContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CheckStatusController(IHttpContextAccessor httpContextAccessor, cachaContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }


        [HttpGet]
        public IActionResult CheckFreeeNameChanged() 
        {
            try { 
            var memberInfoJson = _httpContextAccessor.HttpContext?.Session.GetString(CDictionary.SK_LOINGED_USER);
            var memberInfo = JsonSerializer.Deserialize<ShopMemberInfo>(memberInfoJson);
            int _memberId = memberInfo.MemberId;
            bool? freeNameChangeValue = _context.ShopMemberInfo
                .Where(c => c.MemberId == _memberId)
                .Select(c => c.FreeNameChange)
                .SingleOrDefault();

            var db = _context.ShopMemberInfo.FirstOrDefault(c => c.MemberId == _memberId);
            if (freeNameChangeValue.HasValue) 
            {
                //改過名字
                return new JsonResult(false);
            }
              
            return new JsonResult(true);
            }
            catch (Exception ex)
            {
                // 處理異常情況
                return StatusCode(500, "發生錯誤：" + ex.Message);
            }
        }



    }
}
