﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using prjCatChaOnlineShop.Models;


namespace prjCatChaOnlineShop.Controllers.Api
{
    [Route("api/Api/[controller]")]
    [ApiController]
    public class GameApiController : ControllerBase
    {
        private readonly cachaContext _context;
        public GameApiController(cachaContext context)
        {
            _context = context;
        }
        [HttpGet]
        //http://localhost:5090/Api/Api/gameapi
        public IActionResult 轉蛋數據()
        {

            try
            {
                var datas = from p in _context.GameProductTotal
                            where p.LotteryProbability != null && p.ProductCategoryId != 2 && p.ProductImage != null
                            select new
                            {
                                p.ProductName,
                                p.ProductId,
                                p.ProductCategoryId,
                                p.LotteryProbability,
                                p.ProductImage,
                                p.CouponId
                            };
                if (datas.Any())
                {
                    return new JsonResult(datas);
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

        //public IActionResult 轉蛋數據2()
        //{
        //    var datas = _context.GameProductTotal.Select(c => c.LotteryProbability).Distinct();
        //    return new JsonResult(datas);
        //}
    }
}
