using prjCatChaOnlineShop.Models;

namespace prjCatChaOnlineShop.Areas.AdminCMS.Models
{
    public class CCase
    {
        public int ComplaintCaseId { get; set; }

        public int? ComplaintStatusId { get; set; }

        public int? ComplaintCategoryId { get; set; }

        public int ReceiverIdOfficial { get; set; }

        public string? MessageRecipientContent { get; set; }

        public DateTime SentTime { get; set; }
    }
}
