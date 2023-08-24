using prjCatChaOnlineShop.Models;

namespace prjCatChaOnlineShop.Models.CModels
{
    public class CProductReviewsWrap
    {
        private ShopProductReviewTable _reviews;
        public ShopProductReviewTable Reviews 
        { 
            get { return _reviews; }
            set { _reviews = value; }
        }
        public int? MemberId
        {
            get { return _reviews.MemberId; }
            set { _reviews.MemberId = value; }
        }

        public int? ProductId 
        {
            get { return _reviews.ProductId; }
            set { _reviews.ProductId = value;}
        }

        public string ReviewContent 
        {
            get { return _reviews.ReviewContent;}
            set { _reviews.ReviewContent = value;}
        }

        public int? ProductRating 
        {
            get { return _reviews.ProductRating;}
            set { _reviews.ProductRating = value;}
        }

        public int ProductReviewId 
        {
            get { return _reviews.ProductReviewId; }
            set { _reviews.ProductReviewId = value;}
        }

        public DateTime? ReviewTime 
        {
            get { return _reviews.ReviewTime;}
            set { _reviews.ReviewTime = value;}
        }

        public bool? HideReview 
        {
            get { return _reviews.HideReview;}
            set { _reviews.HideReview = value;}
        }

        public virtual ShopMemberInfo Member 
        {
            get { return _reviews.Member; }
            set { _reviews.Member = value;}
        }

        public virtual ShopProductTotal Product
        {
            get { return _reviews.Product; }
            set { _reviews.Product = value; }
        }
    }
}
