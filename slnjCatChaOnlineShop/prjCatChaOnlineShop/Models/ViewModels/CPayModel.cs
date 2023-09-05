namespace prjCatChaOnlineShop.Models.ViewModels
{
    public class CPayModel
    {
        public int subtotal { get; set; }
        public decimal shippingFee { get; set; }
        public decimal finalBonus { get; set; }
        public int finalAmount { get; set; }
        public string paymentMethod { get; set; }
        public string shippingMethod { get; set; }

    }
}
