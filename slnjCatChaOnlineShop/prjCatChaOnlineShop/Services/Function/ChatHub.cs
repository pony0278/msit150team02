using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using prjCatChaOnlineShop.Models.ViewModels;

namespace prjCatChaOnlineShop.Services.Function
{
    public class ChatHub : Hub
    {

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
        public override async Task OnConnectedAsync()
        {
            var username = Context.GetHttpContext().Request.Query["username"];
            await base.OnConnectedAsync();
        }

    }
}
