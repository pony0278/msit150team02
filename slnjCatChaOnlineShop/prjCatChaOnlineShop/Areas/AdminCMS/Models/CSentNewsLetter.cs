using prjCatChaOnlineShop.Models;

namespace prjCatChaOnlineShop.Areas.AdminCMS.Models
{
    public class CSentNewsLetter
    {
        private Newsletter _newsletter;
        public Newsletter NewsLetter
        {
            get { return _newsletter; }
            set { _newsletter = value; }
        }
        public CSentNewsLetter()
        {
            _newsletter = new Newsletter();
        }
        public int? TemplateId { get; set; }

        public string? Subject { get; set; }

        public string? ImageUrl { get; set; }
        public IFormFile? ContentImage { get; set; }

        //public string ContentImage { get; set; }
        public DateTime? SendDate { get; set; }
    }
}
