using prjCatChaOnlineShop.Models;

namespace prjCatChaOnlineShop.Areas.AdminCMS.Models
{
    public class CNewsLetterTemplete
    {
        private NewsletterTemplate _newsletter;
        public NewsletterTemplate NewsLetter
        {
            get { return _newsletter; }
            set { _newsletter = value; }
        }
        public CNewsLetterTemplete()
        {
            _newsletter = new NewsletterTemplate();
        }

        public IFormFile? HeaderImage { get; set; }
        public IFormFile? FooterImage { get; set; }
    }
}
