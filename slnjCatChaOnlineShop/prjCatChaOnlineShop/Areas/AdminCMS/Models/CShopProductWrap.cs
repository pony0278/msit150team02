using prjCatChaOnlineShop.Models;
using System.ComponentModel;

namespace prjCatChaOnlineShop.Areas.AdminCMS.Models
{
    public class CShopProductWrap
    {
        public string? ProductName { get; set; }
        public string? ProductDescription { get; set; }
        public int ProductId { get; set; }
        public bool? PushToShop { get; set; }

        public string? ProductImage1 { get; set; }

        public IFormFile? Image { get; set; }
        public decimal? ProductPrice { get; set; }
        public string? Size { get; set; }

        public string? Weight { get; set; }
        public int? SupplierId { get; set; }
        public int? ProductCategoryId { get; set; }
        public int? RemainingQuantity { get; set; }
        public DateTime? ReleaseDate { get; set; }

        public bool? Discontinued { get; set; }
        public decimal? Discount { get; set; }
        public DateTime? OffDay { get; set; }
    }
}
