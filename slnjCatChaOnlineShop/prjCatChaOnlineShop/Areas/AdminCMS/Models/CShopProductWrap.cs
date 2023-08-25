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
    }
}
