using prjCatChaOnlineShop.Models;

namespace prjCatChaOnlineShop.Areas.AdminCMS.Models
{
    public class CGameProductWrap
    {
        public string? ProductName { get ; set ; }

        public int ProductId { get; set; }

        public int? ProductCategoryId { get; set; }

        public string? ProductDescription { get; set; }

        public int? ProductPrice { get; set; }

        public string? ProductImage { get; set; }

        public string? PurchasedQuantity { get; set; }

        public string? RemainingQuantity { get; set; }
        public decimal? LotteryProbability { get; set; }
        public IFormFile? Image { get; set; }

    }
}
