using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjCatChaOnlineShop.Models;
using prjCatChaOnlineShop.Models.CDictionary;
using prjCatChaOnlineShop.Models.CModels;
using System.Linq;
using System.Reflection;
using System.Text.Json;


namespace prjCatChaOnlineShop.Controllers.Api
{
    [Route("api/Api/[controller]")]
    [ApiController]
    public class TestDBLoginController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly cachaContext _context;

        public TestDBLoginController(IHttpContextAccessor httpContextAccessor, cachaContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }
        //http://localhost:5090/Api/Api/TestDBLogin/玩家資訊數據

        [HttpGet("玩家資訊數據")]
        public IActionResult 玩家資訊數據()
        {
            try
            {
                var memberInfoJson = _httpContextAccessor.HttpContext?.Session.GetString(CDictionary.SK_LOINGED_USER);
                var memberInfo = JsonSerializer.Deserialize<ShopMemberInfo>(memberInfoJson);
                int _memberId = memberInfo.MemberId;

                // 判斷是否存在 MemberId，如果不存在，可以創建一個預設的 GameItemPurchaseRecord
                if (!_context.GameItemPurchaseRecord.Any(g => g.MemberId == _memberId))
                {
                    var gameProductTotalRecords = _context.GameProductTotal.ToList();

                    var defaultitems = _context.GameItemPurchaseRecord;

                    foreach (var gameProductTotalRecord in gameProductTotalRecords)
                    {
                        var defaultItem = new GameItemPurchaseRecord
                        {
                            MemberId = _memberId,
                            ProductId = gameProductTotalRecord.ProductId,
                            QuantityOfInGameItems = 0,
                            ItemName = gameProductTotalRecord.ProductName
                        };
                    }
                    var defaultI = new GameItemPurchaseRecord
                    {
                        MemberId = _memberId,
                        ProductId = 22,
                        QuantityOfInGameItems = 1,
                        ItemName = "初始褐貓"
                    };
                    var db = _context.ShopMemberInfo.FirstOrDefault(p => p.MemberId == _memberId);
                    if (db != null && db.LoyaltyPoints == null)
                    {
                        db.CharacterName = "我愛貓抓抓001";
                        db.CatCoinQuantity = 10000;
                        db.RunGameHighestScore = 0;
                        db.LoyaltyPoints = 0;
                    }
                    else if (db != null && db.LoyaltyPoints != null)
                    {
                        db.CharacterName = "我愛貓抓抓001";
                        db.CatCoinQuantity = 10000;
                        db.RunGameHighestScore = 0;
                    };
                    _context.GameItemPurchaseRecord.Add(defaultI);
                    _context.SaveChanges();
                }
                else
                {
                    var gameProductTotalRecords = _context.GameProductTotal.ToList();
                    var existingProductIds = new HashSet<int>();

                    foreach (var existingRecord in _context.GameItemPurchaseRecord.Where(g => g.MemberId == _memberId))
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
                            MemberId = _memberId,
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
                             where p.MemberId == _memberId
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
            catch (Exception ex)
            {
                // 處理異常情況
                return StatusCode(500, "發生錯誤：" + ex.Message);
            }
        }
        //[HttpPost("玩家登入數據")]
        //public IActionResult 玩家登入數據([FromBody] GameReturnGachaDataModel rgm)
        //{

        //    var memberInfoJson = _httpContextAccessor.HttpContext?.Session.GetString(CDictionary.SK_LOINGED_USER);
        //    var memberInfo = JsonSerializer.Deserialize<ShopMemberInfo>(memberInfoJson);
        //    int _memberId = memberInfo.MemberId;

        //    // 判斷是否存在 MemberId，如果不存在，可以創建一個預設的 GameItemPurchaseRecord
        //    if (!_context.GameItemPurchaseRecord.Any(g => g.MemberId == _memberId))
        //    {
        //        var gameProductTotalRecords = _context.GameProductTotal.ToList();

        //        var defaultitems = _context.GameItemPurchaseRecord;

        //        foreach (var gameProductTotalRecord in gameProductTotalRecords)
        //        {
        //            var defaultItem = new GameItemPurchaseRecord
        //            {
        //                MemberId = _memberId,
        //                ProductId = gameProductTotalRecord.ProductId,
        //                QuantityOfInGameItems = 0,
        //                ItemName = gameProductTotalRecord.ProductName
        //            };
        //        }
        //        var defaultI = new GameItemPurchaseRecord
        //        {
        //            MemberId = _memberId,
        //            ProductId = 22,
        //            QuantityOfInGameItems = 1,
        //            ItemName = "初始褐貓"
        //        };
        //        _context.GameItemPurchaseRecord.Add(defaultI);
        //        _context.SaveChanges();
        //    }
        //    else
        //    {
        //        var gameProductTotalRecords = _context.GameProductTotal.ToList();
        //        var existingProductIds = new HashSet<int>();

        //        foreach (var existingRecord in _context.GameItemPurchaseRecord.Where(g => g.MemberId == _memberId))
        //        {
        //            existingProductIds.Add((int)existingRecord.ProductId);
        //        }

        //        var missingProductIds = new List<int>();

        //        foreach (var gameProductTotalRecord in gameProductTotalRecords)
        //        {
        //            if (!existingProductIds.Contains(gameProductTotalRecord.ProductId))
        //            {
        //                missingProductIds.Add(gameProductTotalRecord.ProductId);
        //            }
        //        }

        //        foreach (var missingProductId in missingProductIds)
        //        {
        //            var defaultItem = new GameItemPurchaseRecord
        //            {
        //                MemberId = _memberId,
        //                ProductId = missingProductId,
        //                QuantityOfInGameItems = 0,
        //                ItemName = gameProductTotalRecords.FirstOrDefault(g => g.ProductId == missingProductId)?.ProductName
        //            };
        //            _context.GameItemPurchaseRecord.Add(defaultItem);
        //        }
        //        _context.SaveChanges();
        //    }

        //    // 執行查詢
        //    var datas = (from p in _context.ShopMemberInfo
        //                 join i in _context.GameItemPurchaseRecord on p.MemberId equals i.MemberId
        //                 where p.MemberId == _memberId
        //                 select new
        //                 {
        //                     p.MemberId,
        //                     p.CharacterName,
        //                     p.CatCoinQuantity,
        //                     p.LoyaltyPoints,
        //                     p.RunGameHighestScore,
        //                     i.ProductId,
        //                     i.QuantityOfInGameItems,
        //                     i.ItemName
        //                 })
        //                 .Distinct()
        //                 .ToList();

        //    // 在這裡繼續處理結果，將集合中的元素合併
        //    // ...

        //    if (datas.Any())
        //    {
        //        // 在這裡繼續處理結果，將集合中的元素合併
        //        var mergedData = datas
        //            .GroupBy(d => new { d.MemberId, d.CharacterName, d.CatCoinQuantity, d.LoyaltyPoints, d.RunGameHighestScore })
        //            .Select(group => new
        //            {
        //                group.Key.MemberId,
        //                group.Key.CharacterName,
        //                group.Key.CatCoinQuantity,
        //                group.Key.LoyaltyPoints,
        //                group.Key.RunGameHighestScore,
        //                GameItemInfo = group.Select(g => new { g.ProductId, g.QuantityOfInGameItems, g.ItemName })
        //            })
        //            .ToList();
        //        return new JsonResult(mergedData);
        //    }
        //    else
        //    {
        //        return NotFound();
        //    }
        //}

        [HttpPost("傳回轉蛋數據")]
        public IActionResult 傳回轉蛋數據([FromBody] GameReturnGachaDataModel rgm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("資料驗證失敗");
            }
            try
            {
                var existingMemberInfo = _context.ShopMemberInfo
                     .FirstOrDefault(record => record.MemberId == rgm.MemberId);
                if (existingMemberInfo != null)
                {
                    existingMemberInfo.CatCoinQuantity = rgm.CatCoinQuantity;
                    existingMemberInfo.LoyaltyPoints = rgm.LoyaltyPoints;
                }
                _context.SaveChanges();
                bool foundCoupon = false;
                foreach (var gachaResult in rgm.GachaResult)
                {
                    var existingRecord = _context.GameItemPurchaseRecord
                        .FirstOrDefault(record => record.MemberId == rgm.MemberId && record.ProductId == gachaResult.productId);

                    if (existingRecord != null)
                    {
                        existingRecord.QuantityOfInGameItems += 1;
                        _context.SaveChanges();
                    }
                    else
                    {
                        var dbItemModel = new GameItemPurchaseRecord
                        {
                            ItemName = gachaResult.productName,
                            MemberId = rgm.MemberId,
                            ProductId = gachaResult.productId,
                            QuantityOfInGameItems = 1
                        };
                        _context.GameItemPurchaseRecord.Add(dbItemModel);
                    }

                    if (gachaResult.productCategoryId == 5)
                    {
                        var existingCoupon = _context.ShopCouponTotal
                            .FirstOrDefault(record =>record.CouponId == gachaResult.couponId);
                        if (existingCoupon != null)
                        {
                            var dbCouponModel = new ShopMemberCouponData
                            {
                                MemberId = rgm.MemberId,
                                CouponId = gachaResult.couponId,
                                CouponStatusId = false,                           
                            }; 
                            _context.ShopMemberCouponData.Add(dbCouponModel);
                            foundCoupon = true; // 找到優惠券
                        }
                        else if(existingCoupon == null)
                        {
                            return Ok(new { message = "沒有這個優惠券" });
                        }
                        else
                        {
                            return Ok(new { message = "其他優惠券錯誤" });
                        }
                    }
                    if (gachaResult.productCategoryId == 6)
                    {
                        var existingCoin = _context.ShopMemberInfo
                            .FirstOrDefault(record => record.MemberId == rgm.MemberId);
                        if (existingCoin != null)
                        {
                            existingCoin.CatCoinQuantity +=50;
                        }
                        _context.SaveChanges();
                    }
                }
                if (foundCoupon)
                {
                    _context.SaveChanges(); // 保存所有的變更
                    return Ok(new { message = "存入優惠券" });
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
