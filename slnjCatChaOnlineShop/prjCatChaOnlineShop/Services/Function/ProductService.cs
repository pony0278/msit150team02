using Azure.Identity;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using prjCatChaOnlineShop.Models;
using prjCatChaOnlineShop.Models.CDictionary;
using prjCatChaOnlineShop.Models.CModels;
using prjCatChaOnlineShop.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace prjCatChaOnlineShop.Services.Function
{
    public class ProductService
    {
        private readonly cachaContext _context;
        private readonly IWebHostEnvironment _host;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CheckoutService _checkoutService;
        public ProductService(cachaContext context, IWebHostEnvironment host, IHttpContextAccessor httpContextAccessor, CheckoutService checkoutService)
        {
            _context = context;
            _host = host;
            _httpContextAccessor = httpContextAccessor;
            _checkoutService = checkoutService;

        }
        public List<CProductItem> getProductItems()
        {
            //var data = (from p in _context.ShopProductTotal.AsEnumerable().ToList()
            //            join s in _context.ShopProductSpecification on p.ProductId equals s.ProductId into spGroup
            //            join i in _context.ShopProductImageTable on p.ProductId equals i.ProductId into imgGroup
            //            join c in _context.ShopProductCategory on p.ProductCategoryId equals c.ProductCategoryId
            //            where p.Discontinued != true
            //            select new CProductItem
            //            {
            //                pItems = p,
            //                p子項目 = spGroup.Select(sp => sp.Specification).ToList(),
            //                p圖片路徑 = imgGroup.Select(img => img.ProductPhoto).ToList(),
            //                pCategoryName = c.CategoryName,
            //                p是否加入收藏 = isFavorited(p.ProductId)
            //            }).ToList();
            var data = _context.ShopProductTotal
            .Where(p => p.Discontinued != true)
            .Select(p => new CProductItem
            {
                pItems = p,
                p子項目 = p.ShopProductSpecification.Select(sp => sp.Specification).ToList(),
                p圖片路徑 = p.ShopProductImageTable.Select(img => img.ProductPhoto).ToList(),
                pCategoryName = p.ProductCategory.CategoryName,
                p是否加入收藏 = _context.ShopFavoriteDataTable
                .Any(f => f.MemberId == GetCurrentMemberId() && f.ProductId == p.ProductId)
            })
            .ToList();


            List<CProductItem> items = data;
            return items;
        }

        //public List<CCategoryItem> getCatProductForEachPage(string? catName, int itemPerPage)
        //{
        //    List<CCategoryItem> categoryItems = new List<CCategoryItem>();
        //    var items = getProductByCategoryName(catName).Take(itemPerPage);
        //    var name = items.FirstOrDefault().pCategoryName;
        //    CCategoryItem c = new CCategoryItem();
        //    c.pItem = items.ToList();
        //    c.categoryName = name;
        //    categoryItems.Add(c);
        //    return categoryItems;
        //}
        public CProductItem getProductById(int? id)
        {
            //var data = (from p in _context.ShopProductTotal.AsEnumerable().ToList()
            //            join s in _context.ShopProductSpecification on p.ProductId equals s.ProductId into spGroup
            //            join i in _context.ShopProductImageTable on p.ProductId equals i.ProductId into imgGroup
            //            join c in _context.ShopProductCategory on p.ProductCategoryId equals c.ProductCategoryId
            //            where p.ProductId == id && p.Discontinued != true
            //            select new CProductItem
            //            {
            //                pItems = p,
            //                p子項目 = spGroup.Select(sp => sp.Specification).ToList(),
            //                p圖片路徑 = imgGroup.Select(img => img.ProductPhoto).ToList(),
            //                pCategoryName = c.CategoryName,
            //                p是否加入收藏 = isFavorited(id)


            //            }).FirstOrDefault();
            //CProductItem item = data;
            //return item;
            var data = _context.ShopProductTotal
            .Where(p => p.ProductId == id && p.Discontinued!=true)
            .Select(p => new CProductItem
            {
                pItems = p,
                p子項目 = p.ShopProductSpecification.Select(sp => sp.Specification).ToList(),
                p圖片路徑 = p.ShopProductImageTable.Select(img => img.ProductPhoto).ToList(),
                pCategoryName = p.ProductCategory.CategoryName,
                p是否加入收藏 = _context.ShopFavoriteDataTable
                    .Any(f => f.MemberId == GetCurrentMemberId() && f.ProductId == id)
            })
            .FirstOrDefault();

            return data;
        }
        public List<CProductItem> getProductByCategoryName(string? categoryName)
        {
            //var data = (from p in _context.ShopProductTotal.AsEnumerable().ToList()
            //            join s in _context.ShopProductSpecification on p.ProductId equals s.ProductId into spGroup
            //            join i in _context.ShopProductImageTable on p.ProductId equals i.ProductId into imgGroup
            //            join c in _context.ShopProductCategory on p.ProductCategoryId equals c.ProductCategoryId
            //            where c.CategoryName == categoryName && p.Discontinued != true
            //            select new CProductItem
            //            {
            //                pItems = p,
            //                p子項目 = spGroup.Select(sp => sp.Specification).ToList(),
            //                p圖片路徑 = imgGroup.Select(img => img.ProductPhoto).ToList(),
            //                pCategoryName = c.CategoryName,
            //                p是否加入收藏 = isFavorited(p.ProductId)


            //            }).ToList();
            //List<CProductItem> items = data;
            //return items;
            var data = _context.ShopProductTotal
            .Where(p => p.ProductCategory.CategoryName == categoryName && p.Discontinued != true)
            .Select(p => new CProductItem
            {
                pItems = p,
                p子項目 = p.ShopProductSpecification.Select(sp => sp.Specification).ToList(),
                p圖片路徑 = p.ShopProductImageTable.Select(img => img.ProductPhoto).ToList(),
                pCategoryName = p.ProductCategory.CategoryName,
                p是否加入收藏 = _context.ShopFavoriteDataTable
                    .Any(f => f.MemberId == GetCurrentMemberId() && f.ProductId == p.ProductId)
            })
            .ToList();

            return data;
        }

        public CDetailsViewModel getDetailsById(int? pId)
        {
            CDetailsViewModel details = new CDetailsViewModel();
            details.selectedProduct = getProductById(pId);
            details.recommands = getProductByCategoryName(getProductById(pId).pCategoryName).Where(p=>p.pId!=pId).ToList();
            details.reviews = getReview(pId);
            return details;
        }

        public List<string>? getOtherAttr(int? id)
        {
            var data = from p in _context.ShopProductTotal.AsEnumerable()
                       select p.ProductSp.Specification;

            List<string> attrs = data.ToList();
            return attrs;
        }


        public decimal priceFinal(decimal? price, decimal? priceSale)
        {
            if (price != null && priceSale != null)
            {
                return (decimal)priceSale;//特價時的金額
            }
            else
            {
                return (decimal)price;//無特價時金額
            }
        }
        public void addCartItem(List<CCartItem> cart, CProductItem prodItem, int count)
        {
            var existingCartItem = cart.FirstOrDefault(item => item.cId == prodItem.pId);
            if (existingCartItem != null)
            {
                //TODO...
                //if (existingCartItem.c剩餘庫存 >= 1)
                existingCartItem.c數量 = existingCartItem.c數量 + count;
            }
            else
            {
                CCartItem cartItem = new CCartItem();
                cartItem.cId = prodItem.pId;
                cartItem.cName = prodItem.pName;
                cartItem.cPrice = priceFinal(prodItem.pPrice, prodItem.p優惠價格);
                cartItem.cImgPath = prodItem.p圖片路徑.FirstOrDefault();
                cartItem.c子項目 = prodItem.p子項目.FirstOrDefault();
                cartItem.c剩餘庫存 = prodItem.p剩餘庫存;
                cartItem.c數量 = count;
                cartItem.c其他子項目 = prodItem.p子項目.Where(i => i != cartItem.c子項目).ToList();
                cart.Add(cartItem);
            }
        }
        public void detailsAddCartItem(List<CCartItem> cart, CProductItem prodItem, string option, int? count)
        {
            var existingCartItem = cart.FirstOrDefault(item => item.cId == prodItem.pId && item.c子項目 == option);
            if (existingCartItem != null)
            {
                if (existingCartItem.c剩餘庫存 >= 1)
                    existingCartItem.c數量 = existingCartItem.c數量 + count;
            }
            else
            {
                CCartItem cartItem = new CCartItem();
                cartItem.cId = prodItem.pId;
                cartItem.cName = prodItem.pName;
                cartItem.cPrice = priceFinal(prodItem.pPrice, prodItem.p優惠價格);
                cartItem.cImgPath = prodItem.p圖片路徑.FirstOrDefault();
                cartItem.c子項目 = option;
                cartItem.c剩餘庫存 = prodItem.p剩餘庫存;
                cartItem.c數量 = count;
                cartItem.c其他子項目 = prodItem.p子項目.Where(i => i != cartItem.c子項目).ToList();
                cart.Add(cartItem);
            }
        }
        //找到會員ID
        public int? GetCurrentMemberId()
        {
            var memberInfoJson = _httpContextAccessor.HttpContext?.Session.GetString(CDictionary.SK_LOINGED_USER);
            if (memberInfoJson != null)
            {
                var memberInfo = JsonSerializer.Deserialize<ShopMemberInfo>(memberInfoJson);
                return memberInfo.MemberId;
            }

            return null;
        }
        public List<CProductReview> getReview(int? pId)
        {


            var data = _context.ShopProductReviewTable
                        .Where(r => r.ProductId == pId && r.HideReview == false)
                        .Take(5)
                        .Select(r => new CProductReview
                        {
                            r會員姓名 = r.Member.Name.Length > 2 ?
                             r.Member.Name.Substring(0, r.Member.Name.Length - 2) + "*" + "*" :
                             r.Member.Name,
                            r會員照片 = r.Member.MemberImage,
                            r內容 = r.ReviewContent,
                            r評分 = r.ProductRating,
                            r評分時間 = r.ReviewTime
                        })
                        .ToList();


            return data;
        }

    }
}
