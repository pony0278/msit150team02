namespace prjCatChaOnlineShop.Areas.AdminCMS.Models
{
    public interface IEmailServiceC
    {
       public void SendEmail(int memberId, string subject, string body);
    }
}
