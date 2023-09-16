using prjCatChaOnlineShop.Models;
using prjCatChaOnlineShop.Models.CModels;
using prjCatChaOnlineShop.Models.ViewModels;

namespace prjCatChaOnlineShop.Models.ViewModels
{
    public class CproductsReviews
    {
        public List<ShopProductReviewTable> reviewTables { get; set; }
        public List<ShopProductCategory> productCategories { get; set; }
        public CProductCategory cproductCategory { get; set; }
    }
}
