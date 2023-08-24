namespace prjCatChaOnlineShop.Models.CModels
{
    public class CProductCategory
    {
        private ShopProductCategory _productCategory = null;
        public ShopProductCategory productCategory
        {
             get { return _productCategory; } 
             set {  _productCategory = value; }
        }

        public int categoryId 
        {
            get { return _productCategory.ProductCategoryId; }
            set { _productCategory.ProductCategoryId = value; }
        }
        public string? categoryName 
        {
            get { return _productCategory.CategoryName; }
            set { _productCategory.CategoryName = value; }
        }
    }
}
