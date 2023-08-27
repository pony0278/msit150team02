﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjCatChaOnlineShop.Models;
using prjCatChaOnlineShop.Models.CModels;

namespace prjCatChaOnlineShop.Controllers
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


        public IActionResult Rank(int id)
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



    }
}
