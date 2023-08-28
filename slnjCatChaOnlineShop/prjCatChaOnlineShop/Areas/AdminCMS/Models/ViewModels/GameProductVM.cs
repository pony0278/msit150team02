using prjCatChaOnlineShop.Models;

namespace prjCatChaOnlineShop.Areas.AdminCMS.Models.ViewModels
{
    public class GameProductVM
    {
        public List<GameProductCategory> GameProductCategory { get; set; }
        public List<GameProductTotal> GameProductTotal { get; set; }
    }
}
