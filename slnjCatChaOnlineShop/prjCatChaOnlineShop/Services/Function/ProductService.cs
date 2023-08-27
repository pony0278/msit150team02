using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using Microsoft.EntityFrameworkCore;
using prjCatChaOnlineShop.Models;
using prjCatChaOnlineShop.Models.CModels;
using prjCatChaOnlineShop.Models.ViewModels;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace prjCatChaOnlineShop.Services.Function
{
    public class ProductService
    {
        private readonly cachaContext _context;

        public ProductService(cachaContext context)
        {
            _context = context;
        }
        public List<CProductItem> getProductItems()
        {
            var data = from p in _context.ShopProductTotal
                       from i in _context.ShopProductImageTable
                       where i.ProductId == p.ProductId
                       select new CProductItem
                       {
                           pItems = p,
                           p圖片路徑 = i.ProductPhoto,
                           pCategoryName=p.ProductCategory.CategoryName,
                           pCategoryImg=p.ProductCategory.CategoryDescription
                           
                           //pId = p.ProductId,
                           //pName = p.ProductName,
                           //pPrice = p.ProductPrice,
                           //pDiscount = p.Discount,
                           //pCategoryId = p.ProductCategoryId,
                           //pCategoryName = p.ProductCategory.CategoryName,
                           //pCategoryImg=p.ProductCategory.CategoryDescription,
                           //p上架時間 = p.ReleaseDate,
                           //p剩餘庫存 = p.RemainingQuantity,
                           //p子項目 = p.Attributes,
                       };
            List<CProductItem> items = data.ToList();
            return items;
        }
        public CProductItem getProductById(int? id)
        {
            var data = from p in _context.ShopProductTotal
                       where p.ProductId == id
                       from i in _context.ShopProductImageTable
                       where i.ProductId == p.ProductId
                       select new CProductItem
                       {
                           pItems = p,
                           p圖片路徑 = i.ProductPhoto,
                           pCategoryName = p.ProductCategory.CategoryName,
                           pCategoryImg = p.ProductCategory.CategoryDescription

                       };
            CProductItem item = data.FirstOrDefault();
            return item;
        }   
        public List<CProductItem> getProductByCategoryName(string? categoryName)
        {
            var data = from p in _context.ShopProductTotal
                       where p.ProductCategory.CategoryName == categoryName
                       from i in _context.ShopProductImageTable
                       where i.ProductId == p.ProductId
                       select new CProductItem
                       {
                           pItems = p,
                           p圖片路徑 = i.ProductPhoto,
                           pCategoryName = p.ProductCategory.CategoryName,
                           pCategoryImg = p.ProductCategory.CategoryDescription
                       };
            List<CProductItem> items = data.ToList();
            return items;
        }
        public CDetailsViewModel getDetailsById(int? id)
        {
            var data = (from p in _context.ShopProductTotal
                       where p.ProductId == id
                       from i in _context.ShopProductImageTable
                       where i.ProductId == p.ProductId 
                       select new CProductItem
                       {
                           pItems = p,
                           p圖片路徑 = i.ProductPhoto,
                           pCategoryName = p.ProductCategory.CategoryName,
                           pCategoryImg = p.ProductCategory.CategoryDescription
                       }).FirstOrDefault();
            if(data.p子項目 != null) {
                var dataAttr = (from p in _context.ShopProductTotal
                           where p.ProductName.Equals(data.pName) && p.Attributes != data.p子項目
                           from i in _context.ShopProductImageTable
                           where i.ProductId == p.ProductId&&i.ProductId!= data.pId
                           select new CAttributesViewModel
                           {
                               其他子項目 = p.Attributes,
                               其他子項目圖片 = i.ProductPhoto
                           }).ToList();
                List<CAttributesViewModel> allAttr =dataAttr;
                CDetailsViewModel items = new CDetailsViewModel();
                items.selectedProduct = data;
                items.attrList = allAttr;
                items.recommands = getProductByCategoryName(data.pCategoryName);
                return items;
            }
            else
            {
                CDetailsViewModel items = new CDetailsViewModel();
                items.selectedProduct = data;
                items.recommands = getProductByCategoryName(data.pCategoryName);
                return items;
            }
        }
        public List<CAttributesViewModel> getOtherAttr(int? id)
        {
            var data = from p in _context.ShopProductTotal
                       where p.ProductId == id
                       from i in _context.ShopProductImageTable
                       where i.ProductId == p.ProductId
                       select new CProductItem
                       {
                           pItems = p,
                           p圖片路徑 = i.ProductPhoto,
                           pCategoryName = p.ProductCategory.CategoryName,
                           pCategoryImg = p.ProductCategory.CategoryDescription
                       };
            var dataAttr = from p in _context.ShopProductTotal
                           where p.ProductName.Equals(data.FirstOrDefault().pName)&& p.Attributes!=data.FirstOrDefault().p子項目
                           select new CAttributesViewModel
                           {
                               其他子項目 = p.Attributes,
                               其他子項目圖片 = null
                           };
           
            List<CAttributesViewModel> attrs=dataAttr.ToList();
            return attrs;
        }


        public decimal price(decimal? price, decimal? priceSale)
        {
            if (priceSale != null)
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
                if (existingCartItem.c剩餘庫存 >= 1)
                    existingCartItem.c數量=existingCartItem.c數量+ count;
            }
            else
            {
                CCartItem cartItem = new CCartItem();
                cartItem.cId = prodItem.pId;
                cartItem.cName = prodItem.pName;
                cartItem.cPrice = price(prodItem.pPrice, prodItem.p優惠價格);
                cartItem.cImgPath = prodItem.p圖片路徑;
                cartItem.c子項目 = prodItem.p子項目;
                cartItem.c剩餘庫存 = prodItem.p剩餘庫存;
                cartItem.c數量 = count;
                cartItem.c其他子項目 = getOtherAttr(cartItem.cId);
                cart.Add(cartItem);
            }
        }
        public void detailsAddCartItem(List<CCartItem> cart, CProductItem prodItem, string option, int count)
        {
            var existingCartItem = cart.FirstOrDefault(item => item.cId == prodItem.pId);
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
                cartItem.cPrice = price(prodItem.pPrice, prodItem.p優惠價格);
                cartItem.cImgPath = prodItem.p圖片路徑;
                cartItem.c子項目 = option;
                cartItem.c剩餘庫存 = prodItem.p剩餘庫存;
                cartItem.c數量 = count;
                cartItem.c其他子項目 = getOtherAttr(cartItem.cId);
                cart.Add(cartItem);
            }
        }

        internal int getIdFromAttribute(string newAttribute)
        {
            var data = (from p in _context.ShopProductTotal
                       where p.Attributes== newAttribute
                       select p.ProductId).FirstOrDefault();
            return data;
        }
    }
}
