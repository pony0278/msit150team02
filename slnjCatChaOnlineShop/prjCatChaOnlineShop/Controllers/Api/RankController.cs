using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjCatChaOnlineShop.Models;

namespace prjCatChaOnlineShop.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class RankController : ControllerBase
    {
        private readonly cachaContext _context;
        public RankController(cachaContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Rank()
        {
            var datas = (from p in _context.ShopMemberInfo
                         where p.RunGameHighestScore.HasValue
                         select new
                         {
                             p.CharacterName,
                             p.RunGameHighestScore,
                         })
                         .OrderByDescending(p => p.RunGameHighestScore)
                         .Take(10)
                         .ToList(); 
            //TODO 這邊之後加入新功能: 如果使用者在前10名，則標示出來，沒有在前10名，則把使用者放在第11行，並標示排名

            // 在這裡處理結果，將集合中的元素合併

            if (datas.Any())
            {
                return new JsonResult(datas);
            }
            else
            {
                return NotFound();
            }
        }
    }
}
