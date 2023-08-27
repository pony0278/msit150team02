using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Experimental.ProjectCache;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.Protocol;
using prjCatChaOnlineShop.Areas.AdminCMS.Models;
using prjCatChaOnlineShop.Models;
using prjCatChaOnlineShop.Services.Function;

namespace prjCatChaOnlineShop.Controllers.Home
{
    //[Route("api/[controller]")]
    //[ApiController]

    public class MembershipController : Controller
    {
        private readonly cachaContext _context;

        //建構子先載入資料
        public MembershipController(cachaContext context)
        {
            _context = context;
        }

        public IActionResult Membership()
        {
            return View();
        }

        /*1.基本資料*/

        //取得帳戶基本資料
        public IActionResult GetMemberInfo()
        {
            try
            {
                var datas = _context.ShopMemberInfo.FirstOrDefault(p => p.MemberId == 1035);

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
                            where address.MemberId == 1
                            orderby address.AddressId descending
                            select new
                            {
                                address.AddressId,
                                address.RecipientAddress
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

        //新增常用地址資料
        public IActionResult CreateCommonAddress(string commonAddress)
        {
            try
            {
                ShopCommonAddressData address = new ShopCommonAddressData();

                address.MemberId = 1;
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

        //取得帳戶優惠券資料
        public IActionResult GetMemberCouponData()
        {
            try
            {
                var query = from coupons in _context.ShopMemberCouponData
                            where coupons.MemberId == 1035 & coupons.CouponStatusId == true
                            orderby coupons.Coupon.ExpiryDate
                            select new
                            {
                                coupons.Coupon.CouponName,
                                coupons.Coupon.CouponContent,
                                coupons.Coupon.ExpiryDate
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


        //更新帳戶基本資料到資料庫
        public IActionResult UpdateMemberInfo(ShopMemberInfo member)
        {
            try
            {
                var memberToUpdate = _context.ShopMemberInfo.FirstOrDefault(m => m.MemberId == 1035);

                memberToUpdate.Name = member.Name;
                memberToUpdate.Password = member.Password;
                memberToUpdate.Email = member.Email;
                memberToUpdate.PhoneNumber = member.PhoneNumber;
                memberToUpdate.Birthday = member.Birthday;

                _context.SaveChanges();

                return Content("修改成功");

            }
            catch (Exception ex)
            {
                return Content("修改失敗：" + ex.Message);
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
                            where order.MemberId == 1035
                            select new
                            {
                                order.OrderId,
                                order.OrderCreationDate,
                                order.RecipientName,
                                order.RecipientAddress,
                                order.RecipientPhone,
                                order.PaymentMethod.PaymentMethodName,
                                order.OrderStatus.StatusName,
                                order.ShopOrderDetailTable
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
                            where order.OrderId == orderid
                            select new
                            {
                                order.Product.ProductId,
                                order.Product.ProductName,
                                order.Product.ProductPrice,
                                order.ProductQuantity,
                                order.Product.Size
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

        //加入購物車
        public IActionResult AddShopCart(int productid, ShopReturnDataTable returnn)
        {
            /*
               try
               {
                   returnn.ReturnDate = DateTime.Now;
                   returnn.ProcessingStatusId = 1;

                   if (int.TryParse(HttpContext.Request.Form["orderId"], out int orderId))
                   {
                       returnn.OrderId = orderId;
                   }
                   if (int.TryParse(HttpContext.Request.Form["reasonId"], out int reasonId))
                   {
                       returnn.ReturnReasonId = reasonId;
                   }

                   _context.ShopReturnDataTable.Add(returnn);
                   _context.SaveChanges();

                   return Content("新增成功");
               }
               catch (Exception ex)
               {
                   return Content(ex.Message);
               }
            */
            return null;
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

        //儲存退換貨到資料庫
        public IActionResult SaveReturn(ShopReturnDataTable returnn)
        {
            try
            {
                returnn.ReturnDate = DateTime.Now;
                returnn.ProcessingStatusId = 1;

                if (int.TryParse(HttpContext.Request.Form["orderId"], out int orderId))
                {
                    returnn.OrderId = orderId;
                }
                if (int.TryParse(HttpContext.Request.Form["reasonId"], out int reasonId))
                {
                    returnn.ReturnReasonId = reasonId;
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
                    comment.MemberId = 4;
                    comment.ProductId = productId;
                    comment.ReviewContent = HttpContext.Request.Form["commentText"];
                    comment.ReviewTime = DateTime.Now;
                    comment.HideReview = false;

                    double startRatingValue;
                    if (double.TryParse(HttpContext.Request.Form["startRating"], out startRatingValue))
                    {
                        int rating = (int)Math.Floor(startRatingValue);
                        comment.ProductRating = rating;
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
                            where q.MemberId == 1035
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
                                p.Product.ProductImage1,
                                q.ProductPhoto
                            };


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

        //取得客訴紀錄
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
        
        public async Task<IActionResult> GetComplaintCase()
        {
            try
            {
                var datas = await _context.ShopMemberComplaintCase
                    .AsNoTracking()
                    .OrderByDescending(p => p.CreationTime)
                    .Where(p => p.MemberId == 1)
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
                    complaint.MemberId = 1;
                    complaint.ComplaintCategoryId = categoryId;
                    complaint.ComplaintStatusId = 1;  // Id = 1是指此案件待處理
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
    }

}
