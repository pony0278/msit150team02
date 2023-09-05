using prjCatChaOnlineShop.Models.ViewModels;

namespace prjCatChaOnlineShop.Models.CModels
{
    public class CProductItem
    {
        public ShopProductTotal? pItems { get; set; }
        GameShopBanner _banr = new GameShopBanner();
        public int pId
        {
            get { return pItems.ProductId; }
            set { pItems.ProductId = value; }
        }
        public string? pName
        {
            get { return pItems.ProductName; }
            set { pItems.ProductName = value; }
        }
        public string? pBrandName
        {
            get {
                
                string input = pName;
                string[] parts = input.Split('|'); // 分割字符串
                string result = parts[0].Trim(); // 取得分割後的第一部分並去除首尾空格
                string brandName = result;
                return brandName; 
                
            }
           
        }
        public decimal? pDiscount
        {
            get { return pItems.Discount; }
            set { pItems.Discount = value; }
        }

        public decimal? pPrice
        {
            get { return pItems.ProductPrice; }
            set { pItems.ProductPrice = value; }
        }

        public int? pCategoryId
        {
            get { return pItems.ProductCategoryId; }
            set { pItems.ProductCategoryId = value; }
        }
        public string? pCategoryName { get; set; }
        public string? pCategoryImg { get; set; }
        public DateTime? p上架時間
        {
            get { return pItems.ReleaseDate; }
            set { pItems.ReleaseDate = value; }
        }

        public int? p剩餘庫存
        {
            get { return pItems.RemainingQuantity; }
            set { pItems.RemainingQuantity = value; }
        }
        public string? p商品描述
        {
            get { return pItems.ProductDescription; }
            set { pItems.ProductDescription = value; }
        }
        public List<string?>? p子項目{ get; set;}
        public decimal? p優惠價格
        {
            get
            {
                if (pItems.Discount < 0)//折數
                    return this.pItems.ProductPrice * this.pItems.Discount;
                else//折扣金額
                    return this.pItems.ProductPrice - this.pItems.Discount;
            }
        }
        public List<string?>? p圖片路徑 { get; set; }
        public bool p是否加入收藏 { get; set; }


        public GameShopBanner banner { get { return _banr; } set { _banr = value; } }
        public string Link { get { return _banr.Link; } set { _banr.Link = value; } }
        public bool? Display { get { return _banr.Display; } set { _banr.Display = value; } }

    }
}
