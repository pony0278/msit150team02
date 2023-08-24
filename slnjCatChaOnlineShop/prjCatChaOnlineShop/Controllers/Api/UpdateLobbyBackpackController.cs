using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjCatChaOnlineShop.Models;
using prjCatChaOnlineShop.Models.CModels;

namespace prjCatChaOnlineShop.Controllers.Api
{
    [Route("api/Api/[controller]")]
    [ApiController]
    public class UpdateLobbyBackpackController : ControllerBase
    {
        private readonly cachaContext _context;
        public UpdateLobbyBackpackController(cachaContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult updateUserBackpack(CPlayerItem nID)
        {
            var dbMilkConut = _context.GameItemPurchaseRecord.FirstOrDefault(p => p.MemberId == nID.fId && p.ProductId == 7);
            if (dbMilkConut != null)
            {
                dbMilkConut.QuantityOfInGameItems += nID.fMilkCount;
                _context.SaveChanges();
            }

            var dbCanConut = _context.GameItemPurchaseRecord.FirstOrDefault(p => p.MemberId == nID.fId && p.ProductId == 8);
            if (dbCanConut != null)
            {
                dbCanConut.QuantityOfInGameItems += nID.fCanCount;
                _context.SaveChanges();
            }
            return Ok(new { message = "數據已成功保存" });
        }


    }
}
