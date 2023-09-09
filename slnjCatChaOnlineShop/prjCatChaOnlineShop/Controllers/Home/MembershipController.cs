using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Experimental.ProjectCache;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.Protocol;
using prjCatChaOnlineShop.Areas.AdminCMS.Models;
using prjCatChaOnlineShop.Models;
using prjCatChaOnlineShop.Models.CDictionary;
using prjCatChaOnlineShop.Models.CModels;
using prjCatChaOnlineShop.Services.Function;
using System.Security.Cryptography;
using System.Text.Json;
using prjCatChaOnlineShop.Controllers.Home;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography.X509Certificates;
using prjCatChaOnlineShop.Models.ViewModels;
using MailKit;

namespace prjCatChaOnlineShop.Controllers.Home
{
    //[Route("api/[controller]")]
    //[ApiController]

    public class MembershipController : SuperController
    {
        private readonly cachaContext _context;
        private readonly ProductService _productService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private int? memberIdForMembership = null;
        private readonly ImageService _imageService;


        //建構子先載入資料
        public MembershipController(cachaContext context, IHttpContextAccessor httpContextAccessor, ProductService productService, ImageService imageService )
        {
            _context = context;
            _productService = productService;
            _httpContextAccessor = httpContextAccessor;
            _imageService = imageService;


            memberIdForMembership = GetCurrentMemberId();
        }


        public IActionResult Membership()
        {
            //傳遞會員資料
            string userName = HttpContext.Session.GetString("UserName");
            ViewBag.UserName = userName;//把使用者名字傳給_Layout
            ViewBag.memberIdForMembership = memberIdForMembership;
            ViewBag.Categories = _productService.getAllCategories();//把類別傳給_Layout

            //傳遞取貨超商信息
            var storename = TempData["storename"];
            var storeaddress = TempData["storeaddress"];
            ViewBag.storename = storename;
            ViewBag.storeaddress = storeaddress;

            return View();
        }

        /*1.基本資料*/

        //取得帳戶基本資料
        public IActionResult GetMemberInfo()
        {

            try
            {
                var datas = _context.ShopMemberInfo.FirstOrDefault(p => p.MemberId == memberIdForMembership);

                if (datas != null)
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
                return Content(ex.Message);
            }
        }

        //取得常用地址資料
        public IActionResult GetCommonAddress()
        {
            try
            {
                var query = from address in _context.ShopCommonAddressData
                            where address.MemberId == memberIdForMembership
                            orderby address.AddressId descending
                            select new
                            {
                                address.AddressId,
                                address.RecipientAddress,
                                address.RecipientName
                            };

                var datas = query.ToList();

                if (datas != null)
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
                return Content(ex.Message);
            }
        }

        //取得常用地址資料
        public IActionResult GetCommonShop()
        {
            try
            {
                var query = from shop in _context.ShopCommonShop
                            join  shopName in _context.ShopCommonshopName on shop.CommShopId equals shopName.ShopId
                            where shop.MemberId == memberIdForMembership
                            orderby shop.Id descending
                            select new
                            {
                                shop.Id,
                                shopName.ShopName,
                                shopName.CityName,
                                shopName.DiscitcName,
                            };

                var datas = query.ToList();

                if (datas != null)
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
                return Content(ex.Message);
            }
        }

        //新增常用地址資料:多個參數版
        public IActionResult CreateCommonAddress(ShopCommonAddressData commonAddress)
        {
            try
            {

                ShopCommonAddressData address = new ShopCommonAddressData();

                address.MemberId = memberIdForMembership;
                address.RecipientName = commonAddress.RecipientName;
                address.RecipientPhone = commonAddress.RecipientPhone;
                address.RecipientAddress = commonAddress.RecipientAddress;

                _context.ShopCommonAddressData.Add(address);
                _context.SaveChanges();

                return Content("新增成功");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        //新增常用地址資料:單個參數版
        /*
        public IActionResult CreateCommonAddress(string commonAddress)
        {
            try
            {
                ShopCommonAddressData address = new ShopCommonAddressData();

                address.MemberId = memberIdForMembership;
                address.RecipientAddress = commonAddress;


                _context.ShopCommonAddressData.Add(address);
                _context.SaveChanges();

                return Content("新增成功");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }
        */

        //刪除常用地址資料
        public IActionResult DeleteCommonAddress(int addressid)
        {
            try
            {
                var addressData = _context.ShopCommonAddressData.FirstOrDefault(f => f.AddressId == addressid);
                if (addressData != null)
                {
                    _context.ShopCommonAddressData.Remove(addressData);
                    _context.SaveChanges();

                    
                    return new JsonResult(addressData);
                }
                else
                {
                    return Content("找不到要刪除的收藏產品");
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //刪除常用地址資料
        public IActionResult DeleteCommonShop(int shopId)
        {
            try
            {
                var shopData = _context.ShopCommonShop.FirstOrDefault(f => f.Id == shopId);
                if (shopData != null)
                {
                    _context.ShopCommonShop.Remove(shopData);
                    _context.SaveChanges();


                    return new JsonResult(shopData);
                }
                else
                {
                    return Content("找不到要刪除的店家");
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //新增取貨超商資料
        public IActionResult AddNewMarket([FromBody] ShopCommonShopModel model)
        {

            try
            {
                // 創建 ShopCommonShop 對象
                ShopCommonShop CommonShop = new ShopCommonShop();
                CommonShop.MemberId = memberIdForMembership;

                // 創建 ShopCommonshopName 對象
                ShopCommonshopName commonShop = new ShopCommonshopName();
                commonShop.ShopName = model.storename;
                commonShop.CityName = model.storeaddress.Substring(0, 3);
                commonShop.DiscitcName = model.storeaddress.Substring(3);

                // 向 ShopCommonshopName 表中添加 commonShop
                _context.ShopCommonshopName.Add(commonShop);
                _context.SaveChanges(); // 儲存以獲得識別規格的值

                // 獲取 commonShop 的識別規格值
                int commonShopId = commonShop.ShopId;

                // 將識別規格值賦值給 CommonShop 的 ID 屬性
                CommonShop.CommShopId = commonShopId;

                // 向 CommonShop 表中添加 CommonShop
                _context.ShopCommonShop.Add(CommonShop);
                _context.SaveChanges();

                return Content("新增成功");
                
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }

        }

        //取得可使用優惠券資料
        public IActionResult GetMemberCouponData()
        {
            try
            {
                var query = from coupons in _context.ShopMemberCouponData
                            join q in _context.ShopCouponTotal on coupons.CouponId equals q.CouponId
                            where coupons.MemberId == memberIdForMembership
                            & coupons.CouponStatusId == false & q.Usable == true & q.ExpiryDate >= DateTime.UtcNow
                            orderby q.CouponName
                            select new
                            {
                                coupons.Coupon.CouponName,
                                coupons.Coupon.CouponContent,
                                coupons.Coupon.ExpiryDate
                            };

                var groupedData = query.GroupBy(item => new { item.CouponName, item.CouponContent, item.ExpiryDate })
                                       .Select(group => new
                                       {
                                           CouponName = group.Key.CouponName,
                                           CouponContent = group.Key.CouponContent,
                                           ExpiryDate = group.Key.ExpiryDate,
                                           Count = group.Count() 
                                       })
                                       .ToList();

                if (groupedData != null)
                {
                    return new JsonResult(groupedData);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        //取得已失效優惠券資料
        public IActionResult GetInvalidCouponData()
        {
            try
            {
                var query = from coupons in _context.ShopMemberCouponData
                            join q in _context.ShopCouponTotal on coupons.CouponId equals q.CouponId
                            where coupons.MemberId == memberIdForMembership & (coupons.CouponStatusId == true  
                            || q.ExpiryDate < DateTime.UtcNow || q.Usable == false)
                            orderby q.CouponName
                            select new
                            {
                                coupons.Coupon.CouponName,
                                coupons.Coupon.CouponContent,
                                coupons.Coupon.ExpiryDate
                            };

                var groupedData = query.GroupBy(item => new { item.CouponName, item.CouponContent, item.ExpiryDate })
                                       .Select(group => new
                                       {
                                           CouponName = group.Key.CouponName,
                                           CouponContent = group.Key.CouponContent,
                                           ExpiryDate = group.Key.ExpiryDate,
                                           Count = group.Count()
                                       })
                                       .ToList();

                if (groupedData != null)
                {
                    return new JsonResult(groupedData);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        //更新帳戶基本資料到資料庫
        public IActionResult UpdateMemberInfo(ShopMemberInfo member)
        {
            try
            {
                var memberToUpdate = _context.ShopMemberInfo.FirstOrDefault(m => m.MemberId == memberIdForMembership);

                memberToUpdate.Password = member.Password;//
                memberToUpdate.Name = member.Name;//
                memberToUpdate.Birthday = member.Birthday;//
                memberToUpdate.PhoneNumber = member.PhoneNumber;//
                memberToUpdate.Subscribe = member.Subscribe;
                memberToUpdate.Address = member.Address;//

                _context.SaveChanges();

                return Content("修改成功");

            }
            catch (Exception ex)
            {
                return Content("修改失敗：" + ex.Message);
            }
        }

        //前往圖片審核的頁面
        public IActionResult ImageModerator()
        {
            //傳遞會員資料
            string userName = HttpContext.Session.GetString("UserName");
            ViewBag.UserName = userName;//把使用者名字傳給_Layout
            ViewBag.memberIdForMembership = memberIdForMembership;
            ViewBag.Categories = _productService.getAllCategories();//把類別傳給_Layout
            return View();
        }

        //圖片審核的圖片網址存入資料庫
        //[HttpPost]
        public IActionResult ImageModeratorToMemberInfo(string imageUrl)
        {
            try
            {
                var memberToUpdate = _context.ShopMemberInfo.FirstOrDefault(m => m.MemberId == memberIdForMembership);
                memberToUpdate.MemberImage = imageUrl;
                _context.SaveChangesAsync();

                //return new JsonResult(datas);
                return Content(imageUrl);
            }
            catch (Exception ex)
            {
                //return BadRequest("Error saving the announcement.");
                return Content(ex.Message);

            }
        }

        /*2.消費紀錄*/

        //取得訂單資訊
        public IActionResult GetOrders()
        {
            /*
            var datas = from p in _context.ShopOrderTotalTable
                        where p.MemberId == 1
                        select new
                        {
                            p.OrderId, //訂單編號
                            p.OrderCreationDate,  //訂單成立日期
                            p.Address, //收款地址
                            p.RecipientName, //收款人
                            p.RecipientPhone,  //收款電話
                            p.ShippingMethod, //付款方式
                        };*/
            //var datas = _context.ShopOrderTotalTable.Where(p => p.MemberId == 4).ToList();

            /*
            var query = from order in _context.ShopOrderTotalTable
                        //join payment in _context.ShopPaymentMethodData on order.PaymentMethodId equals payment.PaymentMethodId
                        where order.MemberId == 4
                        select new
                        {
                            order.OrderId,
                            order.OrderCreationDate,
                            order.RecipientName,
                            order.RecipientAddress,
                            order.RecipientPhone,
                            order.PaymentMethod.PaymentMethodName,
                            order
                        };*/

            try
            {
                var query = from order in _context.ShopOrderTotalTable
                            orderby order.OrderCreationDate descending
                            where order.MemberId == memberIdForMembership
                            select new
                            {
                                order.OrderId,
                                order.OrderCreationDate,
                                order.RecipientName,
                                order.RecipientAddress,
                                order.RecipientPhone,
                                order.PaymentMethod.PaymentMethodName,
                                order.OrderStatus.StatusName,
                                order.ShopOrderDetailTable,
                                order.ResultPrice
                            };
                var datas = query.ToList();

                if (datas != null)
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
                return Content(ex.Message);
            }
        }

        //取得商品種類用訂單編號
        public IActionResult GetProductByOrderId(int orderid)
        {

            try
            {
                var query = from order in _context.ShopOrderDetailTable
                            orderby order.ProductId descending
                            where order.OrderId == orderid
                            select new
                            {
                                order.Product.ProductId,
                                order.Product.ProductName,
                                order.Product.ProductPrice,
                                order.ProductQuantity,
                                order.Product.Size,
                                FirstImage = order.Product.ShopProductImageTable.FirstOrDefault() // 取得第一筆照片
                            };

                var datas = query.ToList();

                if (datas != null)
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
                return Content(ex.Message);
            }
        }

        //加入購物車1:若採用ProductApiController的方式會自動導回shop頁面，故改用寫在下方維持原頁面
        public IActionResult AddToCart(int pId)
        {
            var prodItem = _productService.getProductById(pId);
            var cart = GetCartFromSession();
            _productService.addCartItem(cart, prodItem, 1);
            SaveCart(cart);
            
            return null;
        }
        //加入購物車2
        private void SaveCart(List<CCartItem> cart)
        {
            string json = System.Text.Json.JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString(CDictionary.SK_PURCHASED_PRODUCTS_LIST, json);
        }
        //加入購物車3
        private List<CCartItem> GetCartFromSession()
        {
            string json = "";
            if (HttpContext.Session.Keys.Contains(CDictionary.SK_PURCHASED_PRODUCTS_LIST))
            {
                json = HttpContext.Session.GetString(CDictionary.SK_PURCHASED_PRODUCTS_LIST);
                return System.Text.Json.JsonSerializer.Deserialize<List<CCartItem>>(json);
            }
            else
            {
                return new List<CCartItem>();
            }
        }

        //取得申訴類型放到客服中心的頁面
        public IActionResult GetReturnReasonCategory()
        {
            try
            {
                var datas = _context.ShopReturnReasonDataTable.ToList();


                if (datas != null)
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
                return Content(ex.Message);
            }
        }

        //取得商品的過往評論
        public IActionResult GetCommentByOrderId(int orderid, int productid)
        {

            try
            {
                var query = from comment in _context.ShopProductReviewTable
                            where comment.OrderId == orderid & comment.ProductId == productid & comment.MemberId == memberIdForMembership
                            select new
                            {
                                comment.ReviewContent,
                                comment.ProductRating,
                                comment.ReviewTime
                            };

                var datas = query.ToList();

                if (datas != null)
                {
                    return new JsonResult(datas);
                }
                else
                {
                    //datas = null;
                    return new JsonResult(datas);
                }
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        //儲存退換貨到資料庫
        public IActionResult SaveReturn(ShopReturnDataTable returnn)
        {
            try
            {
                returnn.ReturnDate = DateTime.Now;
                returnn.ProcessingStatusId = 1;
                returnn.ReturnContent = HttpContext.Request.Form["returnTextarea"];
                returnn.ReturnImage = HttpContext.Request.Form["returnImageInput"];

                if (int.TryParse(HttpContext.Request.Form["orderId"], out int orderId))
                {
                    returnn.OrderId = orderId;
                }
                if (int.TryParse(HttpContext.Request.Form["reasonId"], out int reasonId))
                {
                    returnn.ReturnReasonId = reasonId;
                }
                if (int.TryParse(HttpContext.Request.Form["returnProductSelectedValue"], out int returnProductSelectedValue))
                {
                    returnn.ProductId = returnProductSelectedValue;
                }
                if (int.TryParse(HttpContext.Request.Form["returnNumber"], out int returnNumber))
                {
                    returnn.ReturnCount = returnNumber;
                }

                _context.ShopReturnDataTable.Add(returnn);
                _context.SaveChanges();

                return Content("新增成功");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        //儲存評論內容到資料庫
        public IActionResult SaveComment(ShopProductReviewTable comment)
        {
            try
            {
                var productIdValue = HttpContext.Request.Form["productId"];

                if (int.TryParse(productIdValue, out int productId))
                {
                    comment.MemberId = memberIdForMembership;
                    comment.ProductId = productId;
                    comment.ReviewTime = DateTime.Now;
                    comment.HideReview = false;

                    if (string.IsNullOrWhiteSpace(HttpContext.Request.Form["commentText"]))
                    {
                        comment.ReviewContent = null;
                    }
                    else
                    {
                        comment.ReviewContent = HttpContext.Request.Form["commentText"];
                    }

                    decimal startRatingValue;
                    if (decimal.TryParse(HttpContext.Request.Form["startRating"], out startRatingValue))
                    {
                        comment.ProductRating = startRatingValue;
                    }

                    int orderIdValue;
                    if (int.TryParse(HttpContext.Request.Form["orderIdSpan"], out orderIdValue))
                    {
                        comment.OrderId = orderIdValue;
                    }


                    _context.ShopProductReviewTable.Add(comment);
                    _context.SaveChanges();

                    return Content("新增成功");
                }
                else
                {
                    return Content("產品編號轉型有誤");
                }
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        /*3.退貨紀錄*/

        //取得退貨紀錄
        public IActionResult GetReturnRecord()
        {

            try
            {
                var datas = from p in _context.ShopReturnDataTable
                            join q in _context.ShopOrderTotalTable on p.OrderId equals q.OrderId
                            orderby p.ReturnDate descending
                            where q.MemberId == memberIdForMembership
                            select new
                            {
                                p.OrderId,
                                p.ProcessingStatus.StatusName,
                                p.ReturnDate,
                                p.ReturnReason.ReturnReason,
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
                return Content(ex.Message);
            }

        }


        /*4.收藏紀錄*/

        //取得收藏商品
        public IActionResult GetFavoriteData()
        {

            try
            {
                var datas = from p in _context.ShopFavoriteDataTable
                            where p.MemberId == memberIdForMembership
                            orderby p.CreationDate descending
                            select new
                            {
                                p.Product.ProductName,
                                p.Product.ProductPrice,
                                p.Product.RemainingQuantity,
                                p.Product.ProductDescription,
                                p.Product.ProductId,
                                p.FavoriteId,
                                //p.Product.ShopProductImageTable.Count
                                ProductPhoto = p.Product.ShopProductImageTable.FirstOrDefault().ProductPhoto,
                            };


                /*
                var datas = from p in _context.ShopFavoriteDataTable
                            join q in _context.ShopProductImageTable on p.ProductId equals q.ProductId
                            orderby p.CreationDate descending
                            where p.MemberId == 1033
                            select new
                            {
                                p.Product.ProductName,
                                p.Product.ProductPrice,
                                p.Product.RemainingQuantity,
                                p.Product.ProductDescription,
                                p.Product.ProductId,
                                p.FavoriteId,
                                q.ProductPhoto
                            };
                */

                if (datas.Any())
                {
                    return new JsonResult(datas);
                }
                else
                {
                    datas = null;
                    return new JsonResult(datas);
                }
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }

        }

        //移除收藏商品
        public IActionResult DeleteFavoriteProduct(int favoriteId)
        {
            try
            {
                var favoriteProduct = _context.ShopFavoriteDataTable.FirstOrDefault(f => f.FavoriteId == favoriteId);
                if (favoriteProduct != null)
                {
                    _context.ShopFavoriteDataTable.Remove(favoriteProduct);
                    _context.SaveChanges();
                    return Content("移除成功");
                }
                else
                {
                    return Content("找不到要刪除的收藏產品");
                }
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        /*5.客訴紀錄*/

        //取得客訴細節
        public IActionResult GetComplaintCaseDetail(int complaintcaseid)
        {
            
            try
            {
                var datas = from p in _context.ShopReplyData
                            where p.ComplaintCaseId == complaintcaseid
                            select new
                            {
                                p.MessageRecipientContent,
                                p.SentTime
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
                return Content(ex.Message);
            }

        }

        //取得客訴案件
        public async Task<IActionResult> GetComplaintCase()
        {
            try
            {
                var datas = await _context.ShopMemberComplaintCase
                    .AsNoTracking()
                    .OrderByDescending(p => p.CreationTime)
                    .Where(p => p.MemberId == memberIdForMembership)
                    .Select(p => new
                    {
                        p.ComplaintCaseId,
                        p.ComplaintContent,
                        p.ComplaintTitle,
                        p.CreationTime,
                        p.ComplaintStatus.ComplaintStatusName,
                        p.ComplaintCategory.CategoryName,
                    })
                    .ToListAsync();

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
                return Content(ex.Message);
            }
        }

        /*6.客服中心*/

        //取得申訴類型放到客服中心的頁面
        public IActionResult GetAppealCategory()
        {
            try
            {
                var datas = _context.ShopAppealCategoryData.ToList();

                if (datas != null)
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
                return Content(ex.Message);
            }
        }

        //儲存客訴內容到資料庫
        public IActionResult SaveComplaint([Bind("ComplaintTitle,ComplaintContent")] ShopMemberComplaintCase complaint)
        {
            try
            {
                var selectedValue = HttpContext.Request.Form["selectedValue"];
                if (int.TryParse(selectedValue, out int categoryId))
                {
                    complaint.MemberId = memberIdForMembership;
                    complaint.ComplaintCategoryId = categoryId;
                    complaint.ComplaintStatusId = 1;
                    complaint.CreationTime = DateTime.Now;

                    _context.ShopMemberComplaintCase.Add(complaint);
                    _context.SaveChanges();

                    return Content("新增成功");
                }
                else
                {
                    return Content("申訴類型選擇有誤");
                }
            }
            catch (Exception ex)
            {
                return Content("新增失敗：" + ex.Message);
            }
        }

        //取得會員id的方法
        private int? GetCurrentMemberId()
        {
            var memberInfoJson = _httpContextAccessor.HttpContext?.Session.GetString(CDictionary.SK_LOINGED_USER);
            if (memberInfoJson != null)
            {
                var memberInfo = System.Text.Json.JsonSerializer.Deserialize<ShopMemberInfo>(memberInfoJson);
                return memberInfo.MemberId;
            }
            return null;
        }

        
    }

    //用於取得超商api信息
    public class ConvenienceStoreController : Controller
    {
        public IActionResult SlectShop(IFormCollection ShopDetail)
        {

            //var storeid = ShopDetail["storeid"];
            string storename = "7-11" + ShopDetail["storename"];
            string storeaddress = ShopDetail["storeaddress"];

            TempData["storename"] = storename;
            TempData["storeaddress"] = storeaddress;

            //return Content(storeid + "/" + storename + "/" + storeaddress);
            //return RedirectToAction("Index", "Shopping", new { ifRe = storeaddress });
            return RedirectToAction("membership", "membership", null);

        }

        public IActionResult SlectOtherShop(IFormCollection ShopDetail)
        {

            //var storeid = ShopDetail["storeid"];
            //string storename = ShopDetail["storename"];
            //string storeaddress = ShopDetail["storeaddress"];

            //TempData["storename"] = storename;
            //TempData["storeaddress"] = storeaddress;

            //return Content(storeid + "/" + storename + "/" + storeaddress);
            //return RedirectToAction("Index", "Shopping", new { ifRe = storeaddress });
            //return RedirectToAction("membership", "membership", null);
            return Content("hi");
        }
    }
}
