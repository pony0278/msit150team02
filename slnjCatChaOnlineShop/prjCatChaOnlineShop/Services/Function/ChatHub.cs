using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using prjCatChaOnlineShop.Models.ViewModels;
using System.Collections.Concurrent;
using prjCatChaOnlineShop.Models;
using System.Linq;

namespace prjCatChaOnlineShop.Services.Function
{
    public class ChatHub : Hub
    {
        private readonly cachaContext _cachaContext;

        public ChatHub (cachaContext cachaContext)
        {
            _cachaContext = cachaContext;
        }

        private static ConcurrentDictionary<string, string> Users = new ConcurrentDictionary<string, string>();

        public List<BannedUsers> BannedID()
        {
            var data = _cachaContext.ShopMemberInfo
                        .Where(x => x.IsBanned == true)
                        .Select(x => new BannedUsers
                        {
                            Name = x.Name,
                            
                        })
                        .ToList();
            return data;
        }

        public async Task SendMessage(string user, string message)
        {
            // 解碼可能包含HTML實體的用戶名
            string decodedStr = System.Net.WebUtility.HtmlDecode(user);

            // 查詢數據庫以確定該用戶是否被禁言
            var bannedUsers = BannedID();
            bool isBanned = bannedUsers.Any(x => x.Name == decodedStr);

            if (!isBanned)
            {
                // 未被禁言，發送消息
                await Clients.All.SendAsync("ReceiveMessage", user, message);
            }
            else
            {
                // 被禁言，不發送消息，可選地通知該用戶
                await Clients.Caller.SendAsync("ReceiveMessage", "系統", "您已違反社群發言規則，您30分鐘內禁止發言");
            }
        }

        public async Task<string> SetUserName(string userName)
        {
            // 做一些異步操作，例如資料庫存儲等。
            return userName;
        }
    }
}
