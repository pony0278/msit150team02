using NETCore.MailKit.Core;
using prjCatChaOnlineShop.Models;

namespace prjCatChaOnlineShop.Areas.AdminCMS.Models
{
    public class EmailService : IEmailServiceC
    {
        private readonly cachaContext _cachaContext;

        public EmailService(cachaContext cachaContext)
        {
            {
                _cachaContext = cachaContext;
            }
        }
        public void SendEmail(int memberId, string subject, string body)
        {
            // 使用 MemberId 查找對應的 Email
            var member = _cachaContext.ShopMemberInfo.FirstOrDefault(m => m.MemberId == memberId);

            if (member != null)
            {
                // 取得會員的 Email
                string toEmail = member.Email;

                // 實現發送 Email 的邏輯，你可以使用 toEmail 來發送郵件
            }
        }

    }
}
