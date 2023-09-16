using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using prjCatChaOnlineShop.Models;

namespace prjCatChaOnlineShop.Controllers.Api
{
    [Route("api/Api/[controller]")]
    [ApiController]
    public class TestUserLoginController : ControllerBase
    {
        private readonly cachaContext _context;
        public TestUserLoginController(cachaContext context)
        {
            _context = context;
        }
        //http://localhost:5090/Api/Api/TestUserLogin

        [HttpPost("玩家登入數據")]
        public IActionResult 玩家登入數據([FromBody] GameReturnGachaDataModel rgm)
        {
            try
            {
                var player = _context.ShopMemberInfo.FirstOrDefault(p => p.MemberId == rgm.MemberId);
                if (player != null)
                {
                    // 找到玩家
                    return Ok(new { message = "找到這個玩家" });
                }
                else
                {
                    // 玩家不存在，返回自定義錯誤訊息
                    return Ok(new { message = "沒有此玩家" });
                }
            }
            catch (Exception ex)
            {
                // 處理異常情況
                return StatusCode(500, "發生錯誤：" + ex.Message);
            }
        }
        [HttpPost("玩家註冊數據")]
        public IActionResult 玩家註冊數據([FromBody] GameReturnGachaDataModel rgm)
        {
            try
            {
                var user註冊 = new ShopMemberInfo()
                {
                    CharacterName = rgm.CharacterName
                };
                _context.ShopMemberInfo.Add(user註冊);
                _context.SaveChanges();
                return Ok(new { message = "註冊成功" });
            }
            catch (Exception ex)
            {
                // 處理異常情況
                return StatusCode(500, "發生錯誤：" + ex.Message);
            }
        }
    }
}
