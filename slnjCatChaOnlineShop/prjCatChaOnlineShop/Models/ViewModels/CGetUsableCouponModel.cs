namespace prjCatChaOnlineShop.Models.ViewModels
{
    public class CGetUsableCouponModel
    {
        public int CouponID { get; set; }
        public string CouponName { get; set; }
        public string CouponContent { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool? Usable { get; set; }
        public decimal? SpecialOffer { get; set; }
    }
}
