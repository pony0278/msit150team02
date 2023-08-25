using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjCatChaOnlineShop.Models;
using prjCatChaOnlineShop.Models.CModels;

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
        public IActionResult Rank(/*CMemberGameInfo c*/)
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
            //var currentUserData = (from p in _context.ShopMemberInfo
            //                       where p.MemberId == c.fId // 假設您有當前使用者的 ID
            //                       select new
            //                       {
            //                           p.CharacterName,
            //                           p.RunGameHighestScore,
            //                       })
            //           .FirstOrDefault();
           
            //如果抓出來的前十名有包含當前使用者，則他的字要是紅色
            //如果抓出來的前十名不包含當前使用者，則要再多加一個在最下面

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
