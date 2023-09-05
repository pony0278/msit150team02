using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace prjCatChaOnlineShop.Services.Function
{
    public class User
    {
        [Key]
        public string ConnectionID { get; set; }
        public string Name { get; set; }

        public User(string name, string connectionId)
        {
            this.Name = name;
            this.ConnectionID = connectionId;
        }
    }
    public class OneToOneHub : Hub
    {
        public static List<User> users = new List<User>();
        public static string AdminName = "客服人員";

        public async Task SendMessage(string connectionId, string message)
        {
            var currentUser = users.Where(u => u.ConnectionID == Context.ConnectionId).FirstOrDefault();
            var targetUser = users.Where(u => u.ConnectionID == connectionId).FirstOrDefault();

            if (currentUser != null && targetUser != null)
            {
                if (currentUser.Name == AdminName || targetUser.Name == AdminName)
                {
                    await Clients.Client(connectionId).SendAsync("addMessage", message + " " + DateTime.Now, Context.ConnectionId);
                    await Clients.Client(Context.ConnectionId).SendAsync("addMessage", message + " " + DateTime.Now, connectionId);
                }
                else
                {
                    await Clients.Client(Context.ConnectionId).SendAsync("showMessage", "客服人員不在線上。");
                }
            }
            else
            {
                await Clients.Client(Context.ConnectionId).SendAsync("showMessage", "客服人員不在線上");
            }
        }

        public async Task GetName(string name)
        {
            var user = users.SingleOrDefault(u => u.ConnectionID == Context.ConnectionId);
            if (user != null)
            {
                user.Name = name;
                await Clients.Client(Context.ConnectionId).SendAsync("showId", Context.ConnectionId);
            }
            GetUsers();
        }

        public override async Task OnConnectedAsync()
        {
            var user = users.SingleOrDefault(u => u.ConnectionID == Context.ConnectionId);
            if (user == null)
            {
                user = new User("", Context.ConnectionId);
                users.Add(user);
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var user = users.SingleOrDefault(p => p.ConnectionID == Context.ConnectionId);
            if (user != null)
            {
                users.Remove(user);
            }
            GetUsers();
            await base.OnDisconnectedAsync(exception);
        }

        private async Task GetUsers()
        {
            var list = users.Select(s => new { s.Name, s.ConnectionID }).ToList();
            string jsonList = JsonConvert.SerializeObject(list);
            await Clients.All.SendAsync("getUsers", jsonList);
        }
    }
}
