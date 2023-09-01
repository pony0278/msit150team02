using prjCatChaOnlineShop.Models.CModels;

namespace prjCatChaOnlineShop.Models.ViewModels
{
    public class CDetailsViewModel
    {
        public CProductItem? selectedProduct { get; set; }
        public List<CProductItem>? recommands { get; set; }
        public List<CProductReview>? reviews { get; set; }
    }
}
