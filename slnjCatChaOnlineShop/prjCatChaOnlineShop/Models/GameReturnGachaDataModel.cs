namespace prjCatChaOnlineShop.Models
{
    public partial class GameReturnGachaDataModel
    {
        public int MemberId { get; set; }
        public int CatCoinQuantity { get; set; }
        public int LoyaltyPoints { get; set; }
        public List<int>? ProductIds { get; set; }
    }
}
