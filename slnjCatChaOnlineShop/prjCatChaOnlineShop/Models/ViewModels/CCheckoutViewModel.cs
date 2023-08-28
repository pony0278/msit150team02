using prjCatChaOnlineShop.Models.CModels;

namespace prjCatChaOnlineShop.Models.ViewModels
{
    public class CCheckoutViewModel
    {
        //會員可以用的優惠券
        public List<CGetUsableCouponModel> memberUsableCoupon { get; set; }
        //會員的購物車商品
        public List<CCartItem> cartItems { get; set; }

        //會員儲存的常用地址
        public List<CgetUsableAddressModel> memberUsableAddress { get; set; }
    }
}
