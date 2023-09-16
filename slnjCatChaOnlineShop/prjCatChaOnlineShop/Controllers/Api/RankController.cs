using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjCatChaOnlineShop.Models;
using prjCatChaOnlineShop.Models.CModels;
using System.Linq;
using System.Text.Json;

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
        public IActionResult Rank(int id)
        {
            try
            {
                //先全部抓出來，目的是取得目前使用者的名次
                var datas = (from p in _context.ShopMemberInfo
                             where p.RunGameHighestScore.HasValue
                             orderby p.RunGameHighestScore descending
                             select new
                             {
                                 p.MemberId,
                                 p.CharacterName,
                                 p.RunGameHighestScore,
                             })
                         .ToList();

                var rankedDatas = datas.Select((data, index) => new
                {
                    Rank = index + 1, // 排名是索引 + 1
                    data.MemberId,
                    data.CharacterName,
                    data.RunGameHighestScore
                }).ToList();

                if (rankedDatas.Any())
                {

                    return new JsonResult(rankedDatas);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                // 處理異常情況
                return StatusCode(500, "發生錯誤：" + ex.Message);
            }
        }

    }
}
