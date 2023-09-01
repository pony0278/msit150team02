using Microsoft.AspNetCore.SignalR;
using prjCatChaOnlineShop.Models.ViewModels;

namespace prjCatChaOnlineShop.Services.Function
{
    public class ChatAsync
    {
        private readonly IHubContext<ChatHub> _context;
        public ChatAsync(IHubContext<ChatHub> hubContext)
        {
            _context = hubContext;
        }
        public async Task SendMessageToAllAsync(string user, string message)
        {
            await _context.Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
