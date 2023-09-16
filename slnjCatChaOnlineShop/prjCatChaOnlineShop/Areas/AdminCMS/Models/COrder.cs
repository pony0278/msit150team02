namespace prjCatChaOnlineShop.Areas.AdminCMS.Models
{
    public class COrder
    {
        public int? MemberId { get; set; }

        public int OrderId { get; set; }

        public DateTime? LastUpdateTime { get; set; }

        public string? RecipientAddress { get; set; }

        public string? RecipientName { get; set; }

        public string? RecipientPhone { get; set; }

        public int? OrderStatusId { get; set; }
    }
}
