using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjCatChaOnlineShop.Models;
using prjCatChaOnlineShop.Models.CModels;
using prjCatChaOnlineShop.Models.CDictionary;
using System.Linq;
using System.Text.Json;

namespace prjCatChaOnlineShop.Controllers.Api
{
    [Route("api/Api/[controller]")]
    [ApiController]
    public class UpdateLobbyBackpackController : ControllerBase
    {
        private readonly cachaContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UpdateLobbyBackpackController(IHttpContextAccessor httpContextAccessor, cachaContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }


        [HttpPost]
        public IActionResult updateUserBackpack(CPlayerItem nID)
        {
            try
            {
                var memberInfoJson = _httpContextAccessor.HttpContext?.Session.GetString(CDictionary.SK_LOINGED_USER);
                var memberInfo = JsonSerializer.Deserialize<ShopMemberInfo>(memberInfoJson);
                int _memberId = memberInfo.MemberId;


                var dbMilkConut = _context.GameItemPurchaseRecord.FirstOrDefault(p => p.MemberId == memberInfo.MemberId && p.ProductId == 7);
                if (dbMilkConut != null)
                {
                    dbMilkConut.QuantityOfInGameItems += nID.fMilkCount;
                    _context.SaveChanges();
                }

                var dbCanConut = _context.GameItemPurchaseRecord.FirstOrDefault(p => p.MemberId == memberInfo.MemberId && p.ProductId == 8);
                if (dbCanConut != null)
                {
                    dbCanConut.QuantityOfInGameItems += nID.fCanCount;
                    _context.SaveChanges();
                }
                return Ok(new { message = "數據已成功保存" });
            }
            catch (Exception ex)
            {
                // 處理異常情況
                return StatusCode(500, "發生錯誤：" + ex.Message);
            }
        }


    }
}
