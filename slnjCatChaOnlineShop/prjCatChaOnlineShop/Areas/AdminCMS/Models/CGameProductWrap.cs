using prjCatChaOnlineShop.Models;

namespace prjCatChaOnlineShop.Areas.AdminCMS.Models
{
    public class CGameProductWrap
    {
        private GameProductTotal _gprod = null;
        public GameProductTotal gameproduct { get { return _gprod; } set { _gprod = value; } }
        public CGameProductWrap()
        {
            _gprod = new GameProductTotal();
        }

        public string ProductName { get { return _gprod.ProductName; } set { _gprod.ProductName = value; } }

        public int ProductId { get { return _gprod.ProductId; } set { _gprod.ProductId = value; } }

        public int? ProductCategoryId { get { return _gprod.ProductCategoryId; } set { _gprod.ProductCategoryId = value; } }

        public string ProductDescription { get { return _gprod.ProductDescription; } set { _gprod.ProductDescription = value; } }

        public int? ProductPrice { get { return _gprod.ProductPrice; } set { _gprod.ProductPrice = value; } }

        public string ProductImage { get { return _gprod.ProductImage; } set { _gprod.ProductImage = value; } }

        public string PurchasedQuantity { get { return _gprod.PurchasedQuantity; } set { _gprod.PurchasedQuantity = value; } }

        public string RemainingQuantity { get { return _gprod.RemainingQuantity; } set { _gprod.RemainingQuantity=value; } }

        public decimal? LotteryProbability { get { return _gprod.LotteryProbability; } set { _gprod.LotteryProbability=value; } }

        public virtual ICollection<GameItemPurchaseRecord> GameItemPurchaseRecord { get; set; } = new List<GameItemPurchaseRecord>();

        public virtual GameProductCategory ProductCategory { get; set; }


        public IFormFile? Image { get; set; }
        public List<IFormFile>? ProductPhotos { get; set; }

    }
}
