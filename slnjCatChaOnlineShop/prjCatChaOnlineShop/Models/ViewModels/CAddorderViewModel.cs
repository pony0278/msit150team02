namespace prjCatChaOnlineShop.Models.ViewModels
{
    public class CAddorderViewModel
    {
        public int? MemberId { get; set; }

        public DateTime? OrderCreationDate { get; set; }

        public int? AddressId { get; set; }

        public string RecipientAddress { get; set; }

        public string RecipientName { get; set; }

        public string RecipientPhone { get; set; }

        public int? OrderStatusId { get; set; }

        public int? PaymentMethodId { get; set; }

        public int? CouponId { get; set; }

        public int? ShippingMethodId { get; set; }

        public decimal? ResultPrice { get; set; }
    }
}
