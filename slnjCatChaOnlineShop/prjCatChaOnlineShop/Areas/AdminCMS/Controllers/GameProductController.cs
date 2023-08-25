using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjCatChaOnlineShop.Areas.AdminCMS.Models;
using prjCatChaOnlineShop.Models;
using prjCatChaOnlineShop.Services.Function;

namespace prjCatChaOnlineShop.Controllers.CMS
{
    [Area("AdminCMS")]
    public class GameProductController : Controller
    {
        private readonly ImageService _imageService;
        private readonly cachaContext _cachaContext;

        public GameProductController(ImageService imageService, cachaContext cachaContext)
        {
            _imageService = imageService;
            _cachaContext = cachaContext;
        }

        //載入DataTable資料
        public IActionResult LoadDataTable()
        {
            var data = _cachaContext.GameProductTotal;

            return Json(new { data });
        }

        //編輯商品資料
        [HttpGet]
        public IActionResult EditProduct(int  id)
        {
            //Console.WriteLine($"Received id: {id}");
            //var product = _cachaContext.GameProductTotal
            //          .Include(p => p.ProductCategory)
            //          .FirstOrDefault(p => p.ProductId == id);

            if (id == null)
            {
                return Json(new { success = false, message = "ID不存在" });
            }
            GameProductTotal product = _cachaContext.GameProductTotal
                                                                    .Include(p => p.ProductCategory)
                                                                    .FirstOrDefault(p => p.ProductId == id);

            if (product == null)
            {
                return Json(new { success = false, message = "商品不存在" });
            }
            return Json(new { success = true, data = product });
        
        }

        //編輯後儲存商品資料
        [HttpPost]
        [Consumes("multipart/form-data")]
        public IActionResult UpdateProduct(CGameProductWrap editProduct)
        {
            var image = editProduct.ProductImage;
            var productData = _cachaContext.GameProductTotal.FirstOrDefault(p => p.ProductId == editProduct.ProductId);

            //if (image == null || image.Length == 0)
            //{
            //    return BadRequest("未選擇圖片");
            //}
            //string imageURL;
            //try
            //{
            //    imageURL = await _imageService.UploadImageAsync(image);
            //}
            //catch
            //{

            //    return BadRequest("圖片上傳錯誤.");
            //}

            try
            {
                if (productData != null)
                {
                    productData.ProductId = editProduct.ProductId;
                    productData.ProductName = editProduct.ProductName;
                    productData.ProductDescription = editProduct.ProductDescription;
                    productData.ProductCategory = editProduct.ProductCategory;
                    productData.ProductPrice = editProduct.ProductPrice;
                    productData.PurchasedQuantity = editProduct.PurchasedQuantity;
                    productData.RemainingQuantity = editProduct.RemainingQuantity;
                    productData.LotteryProbability = editProduct.LotteryProbability;
                    productData.ProductImage = editProduct.ProductImage;
                    _cachaContext.SaveChanges();

                    return RedirectToAction("GameProduct", "GameProduct", new { area = "AdminCMS" });
                }
                else
                {
                    return Json(new { success = false, message = "編輯商品的資訊為空。" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "編輯商品失敗：" + ex.Message });
            }
        }

        //新增
        [HttpPost]
        public IActionResult CreateProduct(GameProductTotal newProduct)
        {
            try
            {
                if (newProduct != null) 
                {
                    _cachaContext.GameProductTotal.Add(newProduct);
                    _cachaContext.SaveChanges();

                    return Json(new { success = true, message = "商品新增成功！" });
                }
                else
                {
                    return Json(new { success = false, message = "新增的商品資訊為空。" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "商品新增失敗：" + ex.Message });
            }
        }




        public IActionResult GameProduct()
        {
            return View();
        }


    }
}
