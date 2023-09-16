using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using prjCatChaOnlineShop.Models;
using prjCatChaOnlineShop.Models.CDictionary;
using prjCatChaOnlineShop.Models.CModels;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace prjCatChaOnlineShop.Controllers.Api
{
    [Route("api/Api/[controller]")]
    [ApiController]
    public class CheckCharacterController : ControllerBase
    {
        private readonly cachaContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CheckCharacterController(IHttpContextAccessor httpContextAccessor, cachaContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }
        public IActionResult CheckCharacterNameExist(CMemberGameInfo p)
        {
            var memberInfoJson = _httpContextAccessor.HttpContext?.Session.GetString(CDictionary.SK_LOINGED_USER);
            var memberInfo = JsonSerializer.Deserialize<ShopMemberInfo>(memberInfoJson);
            int _memberId = memberInfo.MemberId;

            bool isExist = _context.ShopMemberInfo != null && _context.ShopMemberInfo.Where(c => c.CharacterName == p.fCharacterName).Count() >= 1;

            var db = _context.ShopMemberInfo.FirstOrDefault(c => c.MemberId == _memberId);
             if (!isExist)
            {
                db.FreeNameChange = true;
                db.CharacterName = p.fCharacterName;
                db.RunGameHighestScore = p.fRunGameHighestScore;
                _context.SaveChanges();
            }
            return new JsonResult(isExist);

        }

       



    }
}
