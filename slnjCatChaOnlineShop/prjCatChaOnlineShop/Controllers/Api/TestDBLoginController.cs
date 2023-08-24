using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjCatChaOnlineShop.Models;

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
            var datas = (from p in _context.ShopMemberInfo
                         join i in _context.GameItemPurchaseRecord on p.MemberId equals i.MemberId
                         where p.MemberId == 1033
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
                         .ToList(); // 轉換為 List

            // 在這裡處理結果，將集合中的元素合併

            if (datas.Any())
            {
                // 在這裡處理結果，將集合中的元素合併
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
                    // 檢查資料庫中是否已存在具有相同 MemberId 和 ProductId 的記錄
                    var existingRecord = _context.GameItemPurchaseRecord
                        .FirstOrDefault(record => record.MemberId == rgm.MemberId &&
                        record.ProductId == gachaResult.productId
                        );

                    if (existingRecord != null)
                    {
                        // 如果存在相同記錄，則執行更新操作
                        existingRecord.ItemName = gachaResult.productName;
                        existingRecord.QuantityOfInGameItems += 1; // 更新其他屬性
                    }
                    else
                    {
                        // 如果不存在相同記錄，則執行新增操作
                        var dbItemModel = new GameItemPurchaseRecord
                        {
                            ItemName =gachaResult.productName,
                            MemberId = rgm.MemberId,
                            ProductId = gachaResult.productId,
                            QuantityOfInGameItems = 1
                        // 設定其他屬性
                    };
                        _context.GameItemPurchaseRecord.Add(dbItemModel);
                    }
                }

                // 更新其他記錄（例如 _context.ShopMemberInfo）
                var existingMemberInfo = _context.ShopMemberInfo
                    .FirstOrDefault(record => record.MemberId == rgm.MemberId);
                if (existingMemberInfo != null)
                {
                    existingMemberInfo.CatCoinQuantity = rgm.CatCoinQuantity;
                    existingMemberInfo.LoyaltyPoints = rgm.LoyaltyPoints;
                }

                _context.SaveChanges(); // 儲存所有更改

                return Ok(new { message = "數據已成功保存" });
            }
            catch (Exception ex)
            {
                // 處理異常情況
                return StatusCode(500, "保存數據時發生錯誤：" + ex.Message);
            }
        }

        //[HttpPost]
        //public IActionResult 傳回玩家資訊數據([FromBody] GameReturnGachaDataModel rgm)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest("資料驗證失敗");
        //    }
        //    try
        //    {
        //        // 創建一個資料庫模型對象，將DTO數據映射到模型

        //        foreach (var productId in rgm.ProductIds)
        //        {
        //            // 檢查資料庫中是否已存在具有相同 MemberId 和 ProductId 的記錄
        //            var existingRecord = _context.GameItemPurchaseRecord
        //                .FirstOrDefault(record => record.MemberId == rgm.MemberId && record.ProductId == productId);

        //            if (existingRecord != null)
        //            {
        //                // 如果存在相同記錄，則執行更新操作
        //                existingRecord.QuantityOfInGameItems += 1; // 更新其他屬性
        //                _context.SaveChanges(); // 儲存更改
        //            }
        //            else
        //            {
        //                // 如果不存在相同記錄，則執行新增操作
        //                var dbItemModel = new GameItemPurchaseRecord
        //                {
        //                    MemberId = rgm.MemberId,
        //                    ProductId = productId,
        //                    // 設定其他屬性
        //                };
        //                _context.GameItemPurchaseRecord.Add(dbItemModel);
        //                var existingRecord2 = _context.ShopMemberInfo
        //                    .FirstOrDefault(record => record.MemberId == rgm.MemberId);
        //                if (existingRecord2 != null)
        //                {
        //                    existingRecord2.MemberId = rgm.MemberId;
        //                    existingRecord2.CatCoinQuantity = rgm.CatCoinQuantity;
        //                    existingRecord2.LoyaltyPoints = rgm.LoyaltyPoints;
        //                    _context.SaveChanges();
        //                }
        //                _context.SaveChanges(); // 儲存更改
        //            }
        //        }

        //        return Ok(new { message = "數據已成功保存" });
        //    }
        //    catch (Exception ex)
        //    {
        //        // 處理異常情況
        //        return StatusCode(600, "保存數據時發生錯誤：" + ex.Message);
        //    }
        //}
    }
}
