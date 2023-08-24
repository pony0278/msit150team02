namespace prjCatChaOnlineShop.Areas.AdminCMS.Models
{
    public class CReturn
    {
        public int OrderId { get; set; }

        public int? ReturnReasonId { get; set; }

        public int? ProcessingStatusId { get; set; }
    }
}
