using prjCatChaOnlineShop.Models.ViewModels;
namespace prjCatChaOnlineShop.Models.CModels
{
    public class CCartItem
    {
        
        public int cId { get; set; }
        public string? cName { get; set; }
        public int? c剩餘庫存 { get; set; }
        public decimal cPrice { get; set; }

        public string? c子項目 { get; set; }
        public string? cImgPath { get; set; }
        public int? c數量 { get; set; }
        public decimal? c小計
        {
            get
            {
                return cPrice * c數量;
            }

        }
        public List<CAttributesViewModel>? c其他子項目 { get; set; }

    }
}
