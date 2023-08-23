﻿using Microsoft.EntityFrameworkCore;
using prjCatChaOnlineShop.Models;
using prjCatChaOnlineShop.Models.CModels;
using System.Collections.Generic;

namespace prjCatChaOnlineShop.Services.Function
{
    public class ProductService
    {
        private readonly cachaContext _context;

        public ProductService(cachaContext context)
        {
            _context = context;
        }
        public List<CProductItem> GetProductItems()
        {
            var data = from p in _context.ShopProductTotal
                       from i in _context.ShopProductImageTable
                       where i.ProductId == p.ProductId
                       select new CProductItem
                       {
                           product = p,
                           //pId = p.ProductId,
                           //pName = p.ProductName,
                           //pPrice = p.ProductPrice,
                           //pDiscount = p.Discount,
                           //pCategory = p.ProductCategory.CategoryName,
                           //p上架時間 = p.ReleaseDate,
                           //p剩餘庫存 = p.RemainingQuantity,
                           //p子項目 = p.Attributes,
                           pImgPath = i.ProductPhoto
                       };
            List<CProductItem> items = data.ToList();
            return items;
        }
        public CProductItem GetProductById(int? id)
        {
            var data = from p in _context.ShopProductTotal
                       where p.ProductId == id
                       from i in _context.ShopProductImageTable
                       where i.ProductId == p.ProductId
                       select new CProductItem
                       {
                           product = p,
                           //pId = p.ProductId,
                           //pName = p.ProductName,
                           //pPrice = p.ProductPrice,
                           //pDiscount = p.Discount,
                           //pCategory = p.ProductCategory.CategoryName,
                           //p上架時間 = p.ReleaseDate,
                           //p剩餘庫存 = p.RemainingQuantity,
                           //p子項目 = p.Attributes,
                           pImgPath = i.ProductPhoto
                       };
            CProductItem item = data.FirstOrDefault();
            return item;
        }
        public List<CProductItem> GetCategory(int? categoryId)
        {
            var data = from p in _context.ShopProductTotal
                       where p.ProductCategoryId == categoryId
                       from i in _context.ShopProductImageTable
                       where i.ProductId == p.ProductId
                       select new CProductItem
                       {
                           product = p,
                           //pId = p.ProductId,
                           //pName = p.ProductName,
                            //pPrice = p.ProductPrice,
                           //pDiscount = p.Discount,
                           //pCategory = p.ProductCategory.CategoryName,
                           //p上架時間 = p.ReleaseDate,
                           //p剩餘庫存 = p.RemainingQuantity,
                           //p子項目 = p.Attributes,
                           pImgPath = i.ProductPhoto
                       };
            List<CProductItem> items = data.ToList();
            return items;
        }

        public List<CProductItem> GetProductByCategory(int? categoryId)
        {
            var data = from p in _context.ShopProductTotal
                       where p.ProductCategoryId ==categoryId 
                       from i in _context.ShopProductImageTable
                       where i.ProductId == p.ProductId
                       select new CProductItem
                       {
                           product = p,
                           //pId = p.ProductId,
                           //pName = p.ProductName,
                           //pPrice = p.ProductPrice,
                           //pDiscount = p.Discount,
                           //pCategory = p.ProductCategory.CategoryName,
                           //p上架時間 = p.ReleaseDate,
                           //p剩餘庫存 = p.RemainingQuantity,
                           //p子項目 = p.Attributes,
                           pImgPath = i.ProductPhoto
                       };
            List<CProductItem> items = data.ToList();
            return items;
        }

    }
}
