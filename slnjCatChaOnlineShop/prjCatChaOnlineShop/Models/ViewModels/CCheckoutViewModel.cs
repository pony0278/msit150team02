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
        //創建綠界訂單
        public Dictionary<string, string> keyValuePairs { get; set; }
        //取得會員目前可折抵之紅利
        public CGetCouponPrice getCouponPrice { get; set; }
        //取得會員訂單各金額的計算
        public CPayModel getFinalPriceData { get; set; }
        //取得會員最後選擇的付款方式、運送方式
        public CShippmentModel getFinalShippmentData { get; set;}
        
    }
}
