using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using prjCatChaOnlineShop.Models.ViewModels;
using System.Collections.Concurrent;

namespace prjCatChaOnlineShop.Services.Function
{
    public class ChatHub : Hub
    {
        private static ConcurrentDictionary<string, string> Users = new ConcurrentDictionary<string, string>();
        public async Task SendMessage(string user, string message)
        {
            // 查詢數據庫以確定該用戶是否被禁言
            bool isBanned = (user == "badUser");


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
        public override async Task OnConnectedAsync()
        {
            string username = Context.GetHttpContext().Request.Query["username"];
            Users.TryAdd(Context.ConnectionId, username);
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            string username;
            Users.TryRemove(Context.ConnectionId, out username);

            await base.OnDisconnectedAsync(exception);
        }
    }
}
