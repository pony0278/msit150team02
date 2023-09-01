using prjCatChaOnlineShop.Models;
using System.ComponentModel;

namespace prjCatChaOnlineShop.Areas.AdminCMS.Models
{
    public class CBannerWrap
    {
        public int BannerId { get; set; }

        public string Banner { get; set; }

        public DateTime? PublishDate { get; set; }

        public string Link { get; set; }
        public string ToPage { get; set; }
        public bool? Display { get; set; }
        public IFormFile? Image { get; set; }
    }
}
