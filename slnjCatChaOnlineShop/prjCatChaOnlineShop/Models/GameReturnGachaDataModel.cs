using Microsoft.Build.Experimental.ProjectCache;

namespace prjCatChaOnlineShop.Models
{
    public partial class GameReturnGachaDataModel
    {
        public int MemberId { get; set; }
        public int? CatCoinQuantity { get; set; }
        public int? LoyaltyPoints { get; set; }
        public string? CharacterName { get; set; }
        public List<GachaResult>? GachaResult { get; set; }
    }
    public class GachaResult
    {
        public string? productName { get; set; }
        public int productId { get; set; }
        public int? productCategoryId { get; set; }
        public int? couponId { get; set; }

    }
    //public partial class GameReturnGachaDataModel
    //{
    //    public int MemberId { get; set; }
    //    public int CatCoinQuantity { get; set; }
    //    public int LoyaltyPoints { get; set; }
    //    public string? GachaResult { get; set; }
    ////    public List<GachaResult> GachaResult { get; set; }

    ////    //public List<int>? ProductIds { get; set; }
    ////    //public List<string>? ItemsName { get; set; }
    ////}
    ////public class GachaResult
    ////{
    ////    public List<int>? productId { get; set; }
    ////    public List<string>? productName { get; set; }
    //}
}
