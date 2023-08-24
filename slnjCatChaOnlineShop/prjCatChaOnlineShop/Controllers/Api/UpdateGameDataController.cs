﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjCatChaOnlineShop.Models;
using prjCatChaOnlineShop.Models.CModels;

namespace prjCatChaOnlineShop.Controllers.Api
{
    [Route("api/Api/[controller]")]
    [ApiController]
    public class UpdateGameDataController : ControllerBase
    {
        private readonly cachaContext _context;
        public UpdateGameDataController(cachaContext context)
        {
            _context = context;
        }
        //public IActionResult updateHighestScore(int id) 
        //{
        //    var db = _context.ShopMemberInfo.FirstOrDefault(p => p.MemberId == id);
        //    if (db != null)
        //    {
        //        db.CatCoinQuantity += 100;
        //        //db.LoyaltyPoints = 100;
        //        db.RunGameHighestScore += 10;
        //        _context.SaveChanges();

        //    }
        //    return Ok(new { message = "數據已成功保存" });
        //}

        [HttpPost]
        public IActionResult updateUserBasicData(CMemberGameInfo nID)
        {
            var db = _context.ShopMemberInfo.FirstOrDefault(p => p.MemberId == nID.fId);
            if (db != null )
            {
                db.CatCoinQuantity += nID.fCCoin;
                db.LoyaltyPoints += nID.fRuby;
                db.RunGameHighestScore = nID.fScore;
                _context.SaveChanges();
            }

            return Ok(new { message = "數據已成功保存" });
        }
    }
}
