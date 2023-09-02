namespace prjCatChaOnlineShop.Models.CModels
{
    public class CGetCouponPrice
    {
        //最後可以折抵的紅利
        public int finalBonus { get; set; }
        //最後可折抵的金額
        public decimal finalPrice { get; set; }
    }
}
