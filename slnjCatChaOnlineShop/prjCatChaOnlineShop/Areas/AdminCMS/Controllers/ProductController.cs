using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using prjCatChaOnlineShop.Areas.AdminCMS.Models;
using prjCatChaOnlineShop.Models;
using prjCatChaOnlineShop.Models.CModels;
using prjCatChaOnlineShop.Models.ViewModels;
using prjCatChaOnlineShop.Services.Function;
using System.Drawing;
using System.Text;
using OpenAI_API;
using OpenAI_API.Completions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using ChatGPT.Net.DTO.ChatGPTUnofficial;
using OfficeOpenXml;

namespace prjCatChaOnlineShop.Controllers.CMS
{
    [Area("AdminCMS")]
    public class ProductController : Controller
    {
        private readonly ImageService _imageService;
        private readonly cachaContext _cachaContext;
        private readonly IConfiguration _configuration;

        public ProductController(ImageService imageService, cachaContext cachaContext, IConfiguration configuration)
        {
            _imageService = imageService;
            _cachaContext = cachaContext;
            _configuration = configuration;
        }

        //上傳圖片
        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                return BadRequest("未選擇圖片");
            }

            string imageUrl;
            try
            {
                imageUrl = await _imageService.UploadImageAsync(image);
            }
            catch
            {
                return BadRequest("圖片上傳失敗");
            }
            return Ok(new { imageUrl = $"{imageUrl}" });
        }


        //db載入資料表
        public IActionResult LoadDatatable()
        {
            var data = _cachaContext.ShopProductTotal.Select(x => new
            {
                ProductId = x.ProductId,
                ProductName = x.ProductName,
                ProductDescription = x.ProductDescription.Length > 20 ? x.ProductDescription.Substring(0, 20) : x.ProductDescription,
                ProductPrice = x.ProductPrice == null ? "沒有資料" :
                                            x.ProductPrice.ToString(),
                RemainingQuantity = x.RemainingQuantity == null ? "沒有資料" :
                                                         x.RemainingQuantity.ToString(),
                ProductCategory = x.ProductCategory == null ? "沒有資料" :
                                                     x.ProductCategory.CategoryName.ToString(),
                ShopProductImageTable = x.ShopProductImageTable == null ? "沒有資料" :
                                                                    x.ShopProductImageTable.FirstOrDefault().ProductPhoto,
                ReleaseDate = x.ReleaseDate == null ? "沒有資料" :
                                            x.ReleaseDate.ToString(),
                Size = x.Size == null ? "沒有資料" :
                            x.Size.ToString(),
                Weight = x.Weight == null ? "沒有資料" :
                                   x.Weight.ToString(),
                Supplier = x.Supplier == null ? "沒有資料" :
                                     x.Supplier.CompanyName.ToString(),
                Discontinued = x.Discontinued == null ? "未設定" : (x.Discontinued == true ? "是" : "否")
            }).ToList();
            return Json(new { data });
        }




        //編輯
        [HttpGet]
        public IActionResult EditShopProducts(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Invalid ID" });
            }
            ShopProductTotal cShopProductTotal = _cachaContext.ShopProductTotal
                                                .Include(x => x.Supplier)
                                                .Include(x => x.ShopProductImageTable)
                                                .Include(x=>x.ShopProductSpecification)
                                                .FirstOrDefault(p => p.ProductId == id);
            if (cShopProductTotal == null)
            {
                return Json(new { success = false, message = "Item not found" });
            }
            return Json(new { success = true, data = cShopProductTotal });
        }

        //儲存編輯
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> EditShopProducts([FromForm] CShopProductWrap cShopproduct)
        {
            var image = cShopproduct.Image;
            var editProduct = _cachaContext.ShopProductTotal
                .FirstOrDefault(p => p.ProductId == cShopproduct.ProductId);

            string imageURL = null;
            if (image != null && image.Length > 0)
            {
                try
                {
                    imageURL = await _imageService.UploadImageAsync(image);
                }
                catch
                {
                    return BadRequest("圖片上傳錯誤.");
                }
            }
            //var replaceSpecification = _cachaContext.ShopProductSpecification
            //                .Where(x => x.ProductId == cShopproduct.ProductId && cShopproduct.productSpecificationID.Contains(x.Id))
            //                .ToList();

            //var targetSpecification = replaceSpecification.FirstOrDefault(x => x.Id == cShopproduct.productSpecificationIDforEdit);

            //if (targetSpecification != null)
            //{
            //    targetSpecification.Specification = cShopproduct.ProductSpecificationName ?? targetSpecification.Specification; // null check
            //    _cachaContext.Update(targetSpecification);
            //    try
            //    {
            //        await _cachaContext.SaveChangesAsync();
            //    }
            //    catch (Exception ex)
            //    {
            //        return BadRequest("儲存錯誤");
            //    }
            //}

            var insertImgList = _cachaContext.ShopProductImageTable
                                .Where(x => x.ProductId == cShopproduct.ProductId && cShopproduct.ProductImageID.Contains(x.ProductImageId))
                                .ToList();
            if (insertImgList.Any() && cShopproduct.FrontCover != null)
            {
                var targetImg = insertImgList.FirstOrDefault(x => x.ProductImageId == cShopproduct.ProductImageIDforFrontCover);
                targetImg.FrontCover = cShopproduct.FrontCover;
                _cachaContext.Update(targetImg);
                _cachaContext.SaveChanges();
            }
            List<string> imageUrls = new List<string>();
            if (cShopproduct.ProductPhotos != null && cShopproduct.ProductPhotos.Count > 0)
            {
                for (int i = 0; i < cShopproduct.ProductPhotos.Count; i++)
                {
                    try
                    {
                        var uploadedImageUrl = await _imageService.UploadImageAsync(cShopproduct.ProductPhotos[i]);
                        imageUrls.Add(uploadedImageUrl);

                        if (i < insertImgList.Count)
                        {
                            insertImgList[i].ProductPhoto = uploadedImageUrl;
                        }
                        else
                        {
                            var newImageItem = new ShopProductImageTable
                            {
                                ProductId = cShopproduct.ProductId,
                                ProductPhoto = uploadedImageUrl
                            };
                            _cachaContext.ShopProductImageTable.Add(newImageItem);
                            _cachaContext.SaveChanges();
                        }
                    }
                    catch
                    {
                        return BadRequest("圖片上傳錯誤.");
                    }
                }
            }


            if (editProduct != null)
            {
                if (imageURL != null)
                    editProduct.ProductImage1 = imageURL;
                if (cShopproduct.PushToShop != null)
                    editProduct.PushToShop = cShopproduct.PushToShop;
                if (cShopproduct.ProductName != null)
                    editProduct.ProductName = cShopproduct.ProductName;
                if (cShopproduct.ProductDescription != null)
                    editProduct.ProductDescription = cShopproduct.ProductDescription;
                if (cShopproduct.ProductPrice != null)
                    editProduct.ProductPrice = cShopproduct.ProductPrice;
                if (cShopproduct.ProductCategoryId != null)
                    editProduct.ProductCategoryId = cShopproduct.ProductCategoryId;
                if (cShopproduct.Discontinued != null)
                    editProduct.Discontinued = cShopproduct.Discontinued;
                if (cShopproduct.Discount != null)
                    editProduct.Discount = cShopproduct.Discount;
                if (cShopproduct.ReleaseDate != null)
                    editProduct.ReleaseDate = cShopproduct.ReleaseDate;
                if (cShopproduct.RemainingQuantity != null)
                    editProduct.RemainingQuantity = cShopproduct.RemainingQuantity;
                if (cShopproduct.Size != null)
                    editProduct.Size = cShopproduct.Size;
                if (cShopproduct.Weight != null)
                    editProduct.Weight = cShopproduct.Weight;
                if (cShopproduct.OffDay != null)
                    editProduct.OffDay = cShopproduct.OffDay;
                if (cShopproduct.SupplierId != null)
                    editProduct.SupplierId = cShopproduct.SupplierId;
                _cachaContext.Update(editProduct);
                _cachaContext.SaveChanges();
                return Json(new { success = true, message = "Item updated successfully" });
            }
            return Json(new { success = false, message = "Item not found" });
        }


        //新增
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromForm] CShopProductWrap cShopProduct)
        {
            var NewProduct = new ShopProductTotal
            {
                ProductName = cShopProduct.ProductName,
                ProductDescription = cShopProduct.ProductDescription,
                ProductPrice = cShopProduct.ProductPrice,
                ProductCategoryId = cShopProduct.ProductCategoryId,
                PushToShop = cShopProduct.PushToShop,
                Discount = cShopProduct.Discount,
                ReleaseDate = cShopProduct.ReleaseDate,
                OffDay = cShopProduct.OffDay,
                SupplierId = cShopProduct.SupplierId,
                Size = cShopProduct.Size,
                Weight = cShopProduct.Weight,
                RemainingQuantity = cShopProduct.RemainingQuantity,

            };
            _cachaContext.ShopProductTotal.Add(NewProduct);
            await _cachaContext.SaveChangesAsync();
            List<string> productsSpecification = new List<string>();
            if(cShopProduct.productSpecification !=null && cShopProduct.productSpecification.Count > 0)
            {
                for(var i = 0; i< cShopProduct.productSpecification.Count; i++)
                {
                    try
                    {
                        var addSpecifcation = cShopProduct.productSpecification[i];
                        var newproductSpecification = new ShopProductSpecification
                        {
                            ProductId = NewProduct.ProductId,
                            Specification = addSpecifcation
                        };
                        _cachaContext.ShopProductSpecification.Add(newproductSpecification);
                    }
                    catch 
                    {
                        return BadRequest("商品細項儲存錯誤");
                    }
                }
                await _cachaContext.SaveChangesAsync();
            }
            List<string> imageUrls = new List<string>();
            if (cShopProduct.ProductPhotos != null && cShopProduct.ProductPhotos.Count > 0)
            {
                for (var i = 0; i < cShopProduct.ProductPhotos.Count; i++)
                {
                    try
                    {
                        var uploadedImageUrl = await _imageService.UploadImageAsync(cShopProduct.ProductPhotos[i]);
                        var newImageItem = new ShopProductImageTable
                        {
                            ProductId = NewProduct.ProductId,
                            ProductPhoto = uploadedImageUrl
                        };
                        _cachaContext.ShopProductImageTable.Add(newImageItem);
                    }
                    catch
                    {
                        return BadRequest("圖片上傳錯誤.");
                    }
                }
                await _cachaContext.SaveChangesAsync();
            }
            return Json(new { success = true, message = "Content saved!" });
        }
        [HttpPost]
        public IActionResult SaveSuppier([FromBody]CSuppiersWrap cSuppiers)
        {
            var newSuppiers = new ShopProductSupplier
            {
                CompanyName = cSuppiers.CompanyName,
                ContactPhone = cSuppiers.ContactPhone,
                CompanyAddress = cSuppiers.CompanyAddress,
            };
            _cachaContext.ShopProductSupplier.Add(newSuppiers);
            _cachaContext.SaveChanges();
            return Json(new { success = true, message = "Content saved!" });
        }


        public IActionResult Product()
        {
            //判斷是否有登入
            if (HttpContext.Session.Keys.Contains(CAdminLogin.SK_LOGINED_USER))
            {
                // 讀取管理員姓名
                string adminName = HttpContext.Session.GetString("AdminName");

                // 將管理員姓名傳給view
                ViewBag.AdminName = adminName;

                var product = new CproductsTotal
                {
                    ProductCategories = _cachaContext.ShopProductCategory.ToList(),
                    shopProductTotals = _cachaContext.ShopProductTotal.ToList(),
                    Suppliers = _cachaContext.ShopProductSupplier.ToList(),
                };
                return View(product);
            }
            return RedirectToAction("Login", "CMSHome");

        }

        [HttpPost]
        public async Task<ActionResult> GenerateArticle([FromForm] Dictionary<string, string> keywords)
        {
            try
            {
                string keyword1 = keywords.GetValueOrDefault("keyword1", "Keyword1");
                string keyword2 = keywords.GetValueOrDefault("keyword2", "Keyword2");
                string keyword3 = keywords.GetValueOrDefault("keyword3", "Keyword3");

                var prompt = $"這是3個商品的關鍵字{keyword1}，{keyword2}，{keyword3}。請幫我產生70字左右的商品介紹,可以使用條列式";

                var apiKey = _configuration["OpenAI_API_Key"];

                var openai = new OpenAIAPI(apiKey);
                var completionRequest = new CompletionRequest
                {
                    Prompt = prompt,
                    Model = OpenAI_API.Models.Model.DavinciText,
                    MaxTokens = 500
                };

                var completions = await openai.Completions.CreateCompletionAsync(completionRequest);

                StringBuilder sb = new StringBuilder();
                foreach (var completion in completions.Completions)
                {
                    sb.Append(completion.Text);
                }
                string outputResult = sb.ToString();

                return Ok(new { article = outputResult });
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }
        [HttpPost]
        public async Task<IActionResult> DeleteEditImg([FromBody]int? id)
        {
            if (id == null)
            {
                return BadRequest("Id cannot be null.");
            }

            ShopProductImageTable imageTable = await _cachaContext.ShopProductImageTable.FirstOrDefaultAsync(x => x.ProductImageId == id);

            if (imageTable == null)
            {
                return NotFound("Image not found.");
            }

            _cachaContext.ShopProductImageTable.Remove(imageTable);
            await _cachaContext.SaveChangesAsync();

            return Ok(new { success = true, message = "Content saved!" });
        }

        [HttpPost]
        public async Task<IActionResult> editorSpecificationID([FromBody] CShopProductWrap cShopproduct)
        {
            try
            {
                var existingSpecifications = await _cachaContext.ShopProductSpecification
                                    .Where(x => x.ProductId == cShopproduct.ProductId && cShopproduct.productSpecificationID.Contains(x.Id))
                                    .ToListAsync();

                
                List<string> newSpecifications = new List<string>();
                if (cShopproduct.productSpecification != null && cShopproduct.productSpecification.Count > 0)
                {
                    for (var i = 0; i < cShopproduct.productSpecification.Count; i++)
                    {
                        var newSpecification = cShopproduct.productSpecification[i];
                        newSpecifications.Add(newSpecification.ToString());

                        if (i< existingSpecifications.Count)
                        {
                            
                            existingSpecifications[i].Specification = newSpecification;
                        }
                        else
                        {
                            
                            var newSpecEntity = new ShopProductSpecification
                            {
                                ProductId = cShopproduct.ProductId,
                                Specification = newSpecification
                            };
                            await _cachaContext.ShopProductSpecification.AddAsync(newSpecEntity);
                        }
                    }

                    await _cachaContext.SaveChangesAsync();
                    return Json(new { success = true, Message = "成功修改" });
                }

                return BadRequest("未提供商品細項");
            }
            catch (Exception ex)
            {
                return BadRequest($"商品細項儲存錯誤: {ex.Message}");
            }
        }
        public IActionResult UploadExcel()
        {
            return View();
        }
        [HttpPost]
        public IActionResult UploadExcel(IFormFile file)
        {
            using (var package = new ExcelPackage(file.OpenReadStream()))
            {
                var worksheet = package.Workbook.Worksheets[0];  // 讀取第一個工作表
                var rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)  // 從第二行開始讀取（假設第一行是標題）
                {
                    var categoryName = worksheet.Cells[row, 1].Text;  // 分類名稱在第一列
                    var productName = worksheet.Cells[row, 2].Text;  // 商品名稱在第二列

                    // 尋找或新增商品分類
                    var category = _cachaContext.ShopProductCategory
                                    .FirstOrDefault(c => c.CategoryName == categoryName)
                                    ?? new ShopProductCategory { CategoryName = categoryName };

                    if (category.ProductCategoryId == 0)  // 新增分類
                    {
                       _cachaContext.ShopProductCategory.Add(category);
                        _cachaContext.SaveChanges();  // 儲存以獲得 Id
                    }

                    // 新增商品
                    var product = new ShopProductTotal
                    {
                        ProductName = productName,
                        ProductCategoryId = category.ProductCategoryId
                    };
                    _cachaContext.ShopProductTotal.Add(product);
                }
                _cachaContext.SaveChanges();  // 儲存所有新增的商品
            }
            return Json(new { success = true, Message = "成功修改" });
        }
    }
}
