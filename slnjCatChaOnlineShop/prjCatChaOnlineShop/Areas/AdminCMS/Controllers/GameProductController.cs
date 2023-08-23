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
        public IActionResult GameProduct()
        {
            return View();
        }


        //載入DataTable資料
        public IActionResult LoadDataTable()
        {
            var data = _cachaContext.GameProductTotal;

            return Json(new { data });


            //    var data = _cachaContext.GameProductTotal.Select(x => new
            //    {





            //    }).ToList();

            //    return Json(new { data });
        }

        //編輯商品資料
        [HttpGet]
        public IActionResult EditProduct(int id)
        {
            Console.WriteLine($"Received id: {id}");
            var product = _cachaContext.GameProductTotal
                      .Include(p => p.ProductCategory.CategoryName)
                      .FirstOrDefault(p => p.ProductId == id);

            
            //GameProductTotal product = _cachaContext.GameProductTotal.FirstOrDefault(p => p.ProductId == id);

            if (product != null)
            {
                return Json(new { data = product });
            }
            else
            {
                return NotFound();
            }
        }

        //編輯後儲存商品資料
        [HttpPost]
        public IActionResult UpdateProduct(CGameProductWrap editProduct)
        {

            var productData = _cachaContext.GameProductTotal.FirstOrDefault(p => p.ProductId == editProduct.ProductId);
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

                    return Json(new { success = true, message = "編輯會員成功！" });
                }
                else
                {
                    return Json(new { success = false, message = "編輯會員的資訊為空。" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "編輯會員失敗：" + ex.Message });
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

                    return Json(new { success = true, message = "會員新增成功！" });
                }
                else
                {
                    return Json(new { success = false, message = "新增的會員資訊為空。" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "會員新增失敗：" + ex.Message });
            }
        }




    }
}
