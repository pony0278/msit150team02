﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjCatChaOnlineShop.Models;
using System.Linq;

namespace prjCatChaOnlineShop.Controllers.Api
{
    [Route("api/Api/[controller]")]
    [ApiController]
    public class TestDBLoginController : ControllerBase
    {
        private readonly cachaContext _context;
        public TestDBLoginController(cachaContext context)
        {
            _context = context;
        }
        //http://localhost:5090/Api/Api/TestDBLogin

        [HttpGet]
        public IActionResult 玩家資訊數據()
        {
            var memberId = 1034; // 預設的 MemberId，您可以根據需要進行更改

            // 判斷是否存在 MemberId，如果不存在，可以創建一個預設的 GameItemPurchaseRecord
            if (!_context.GameItemPurchaseRecord.Any(g => g.MemberId == memberId))
            {
                var gameProductTotalRecords = _context.GameProductTotal.ToList();

                var defaultitems = _context.GameItemPurchaseRecord;

             foreach (var gameProductTotalRecord in gameProductTotalRecords)
                {
                    var defaultItem = new GameItemPurchaseRecord
                    {
                        MemberId = memberId,
                        ProductId = gameProductTotalRecord.ProductId,
                        QuantityOfInGameItems = 0, 
                        ItemName = gameProductTotalRecord.ProductName
                    };
                }
                var defaultI = new GameItemPurchaseRecord
                {
                    MemberId = memberId,
                    ProductId = 22,
                    QuantityOfInGameItems = 1,
                    ItemName = "初始褐貓"
                };
                _context.GameItemPurchaseRecord.Add(defaultI);
                _context.SaveChanges(); 
            }
            else
            {
                var gameProductTotalRecords = _context.GameProductTotal.ToList();
                var existingProductIds = new HashSet<int>();

                foreach (var existingRecord in _context.GameItemPurchaseRecord.Where(g => g.MemberId == memberId))
                {
                    existingProductIds.Add((int)existingRecord.ProductId);
                }

                var missingProductIds = new List<int>();

                foreach (var gameProductTotalRecord in gameProductTotalRecords)
                {
                    if (!existingProductIds.Contains(gameProductTotalRecord.ProductId))
                    {
                        missingProductIds.Add(gameProductTotalRecord.ProductId);
                    }
                }

                foreach (var missingProductId in missingProductIds)
                {
                    var defaultItem = new GameItemPurchaseRecord
                    {
                        MemberId = memberId,
                        ProductId = missingProductId,
                        QuantityOfInGameItems = 0,
                        ItemName = gameProductTotalRecords.FirstOrDefault(g => g.ProductId == missingProductId)?.ProductName
                    };
                    _context.GameItemPurchaseRecord.Add(defaultItem);
                }
                _context.SaveChanges();
            }

            // 執行查詢
            var datas = (from p in _context.ShopMemberInfo
                         join i in _context.GameItemPurchaseRecord on p.MemberId equals i.MemberId
                         where p.MemberId == memberId
                         select new
                         {
                             p.MemberId,
                             p.CharacterName,
                             p.CatCoinQuantity,
                             p.LoyaltyPoints,
                             p.RunGameHighestScore,
                             i.ProductId,
                             i.QuantityOfInGameItems,
                             i.ItemName
                         })
                         .Distinct()
                         .ToList();

            // 在這裡繼續處理結果，將集合中的元素合併
            // ...

            if (datas.Any())
            {
                // 在這裡繼續處理結果，將集合中的元素合併
                var mergedData = datas
                    .GroupBy(d => new { d.MemberId, d.CharacterName, d.CatCoinQuantity, d.LoyaltyPoints, d.RunGameHighestScore })
                    .Select(group => new
                    {
                        group.Key.MemberId,
                        group.Key.CharacterName,
                        group.Key.CatCoinQuantity,
                        group.Key.LoyaltyPoints,
                        group.Key.RunGameHighestScore,
                        GameItemInfo = group.Select(g => new { g.ProductId, g.QuantityOfInGameItems, g.ItemName })
                    })
                    .ToList();
                return new JsonResult(mergedData);
            }
            else
            {
                return NotFound();
            }
        }


        [HttpPost]
        public IActionResult 傳回玩家資訊數據([FromBody] GameReturnGachaDataModel rgm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("資料驗證失敗");
            }
            try
            {
                foreach (var gachaResult in rgm.GachaResult)
                {
                    int productId = gachaResult.productId;

                    var existingRecord = _context.GameItemPurchaseRecord
                        .FirstOrDefault(record => record.MemberId == rgm.MemberId && record.ProductId == productId);

                    if (existingRecord != null)
                    {
                        existingRecord.QuantityOfInGameItems += 1;
                    }
                    else
                    {
                        var dbItemModel = new GameItemPurchaseRecord
                        {
                            ItemName = gachaResult.productName,
                            MemberId = rgm.MemberId,
                            ProductId = productId,
                            QuantityOfInGameItems = 1
                        };
                        _context.GameItemPurchaseRecord.Add(dbItemModel);
                    }

                    if (gachaResult.productCategoryId == 5)
                    {
                        var existingCoupon = _context.ShopMemberCouponData
                            .FirstOrDefault(record => record.MemberId == rgm.MemberId && record.CouponId == gachaResult.couponId);

                        if (existingCoupon == null)
                        {
                            var dbCouponModel = new ShopMemberCouponData
                            {
                                MemberId = rgm.MemberId,
                                CouponId = gachaResult.couponId,
                                CouponStatusId = false,
                            };
                            _context.ShopMemberCouponData.Add(dbCouponModel);
                        }
                    }
                }

                var existingMemberInfo = _context.ShopMemberInfo
                    .FirstOrDefault(record => record.MemberId == rgm.MemberId);
                if (existingMemberInfo != null)
                {
                    existingMemberInfo.CatCoinQuantity = rgm.CatCoinQuantity;
                    existingMemberInfo.LoyaltyPoints = rgm.LoyaltyPoints;
                }

                _context.SaveChanges();

                return Ok(new { message = "數據已成功保存" });
            }
            catch (Exception ex)
            {
                // 處理異常情況
                return StatusCode(500, "保存數據時發生錯誤：" + ex.Message);
            }
        }
    }
}
