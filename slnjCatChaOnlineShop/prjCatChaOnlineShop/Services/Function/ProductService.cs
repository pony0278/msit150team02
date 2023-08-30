using Azure.Identity;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using prjCatChaOnlineShop.Models;
using prjCatChaOnlineShop.Models.CModels;
using prjCatChaOnlineShop.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;
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
            var data = (from p in _context.ShopProductTotal.AsEnumerable().ToList()
                        join s in _context.ShopProductSpecification on p.ProductId equals s.ProductId into spGroup
                        join i in _context.ShopProductImageTable on p.ProductId equals i.ProductId into imgGroup
                        join c in _context.ShopProductCategory on p.ProductCategoryId equals c.ProductCategoryId
                        where p.Discontinued !=true
                        select new CProductItem
                        {
                            pItems = p,
                            p子項目 = spGroup.Select(sp => sp.Specification).ToList(),
                            p圖片路徑 = imgGroup.Select(img => img.ProductPhoto).ToList(),
                            pCategoryName = c.CategoryName,
                        }).ToList();

            List<CProductItem> items = data;
            return items;

        }
        public List<CCategoryItem> getCatProductForEachPage(string? catName, int itemPerPage)
        {
            List<CCategoryItem> categoryItems = new List<CCategoryItem>();
            var items = getProductByCategoryName(catName).Take(itemPerPage);
            var name = items.FirstOrDefault().pCategoryName;
            CCategoryItem c = new CCategoryItem();
            c.pItem = items.ToList();
            c.categoryName = name;
            categoryItems.Add(c);
            return categoryItems;
        }
        public CProductItem getProductById(int? id)
        {
            var data = (from p in _context.ShopProductTotal.AsEnumerable().ToList()
                       where p.ProductId == id
                       join s in _context.ShopProductSpecification on p.ProductId equals s.ProductId into spGroup
                       join i in _context.ShopProductImageTable on p.ProductId equals i.ProductId into imgGroup
                       join c in _context.ShopProductCategory on p.ProductCategoryId equals c.ProductCategoryId
                        where p.Discontinued != true
                        select new CProductItem
                       {
                           pItems = p,
                           p子項目 = spGroup.Select(sp => sp.Specification).ToList(),
                           p圖片路徑 = imgGroup.Select(img => img.ProductPhoto).ToList(),
                            pCategoryName = c.CategoryName,
                        }).FirstOrDefault();
            CProductItem item = data;
            return item;
        }   
        public List<CProductItem> getProductByCategoryName(string? categoryName)
        {
            var data = (from p in _context.ShopProductTotal.AsEnumerable().ToList()
                       join s in _context.ShopProductSpecification on p.ProductId equals s.ProductId into spGroup
                       join i in _context.ShopProductImageTable on p.ProductId equals i.ProductId into imgGroup
                        join c in _context.ShopProductCategory on p.ProductCategoryId equals c.ProductCategoryId
                        where c.CategoryName == categoryName
                        where p.Discontinued != true
                        select new CProductItem
                       {
                           pItems = p,
                           p子項目 = spGroup.Select(sp => sp.Specification).ToList(),
                           p圖片路徑 = imgGroup.Select(img => img.ProductPhoto).ToList(),
                            pCategoryName = c.CategoryName,
                        }).ToList();
            List<CProductItem> items = data;
            return items; 
        }
        
        public CDetailsViewModel getDetailsById(int? id)
        {
                CDetailsViewModel details = new CDetailsViewModel();
                details.selectedProduct = getProductById(id);
                details.recommands = getProductByCategoryName(getProductById(id).pCategoryName);
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
                //TODO...
                //if (existingCartItem.c剩餘庫存 >= 1)
                    existingCartItem.c數量=existingCartItem.c數量+ count;
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
            var existingCartItem = cart.FirstOrDefault(item => item.cId == prodItem.pId&&item.c子項目==option);
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
                cartItem.c其他子項目 = prodItem.p子項目.Where(i=>i!= cartItem.c子項目).ToList();
                cart.Add(cartItem);
            }
        }
    }
}
