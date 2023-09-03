using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using prjCatChaOnlineShop.Models.ViewModels;
using System.Collections.Concurrent;
using prjCatChaOnlineShop.Models;
using System.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace prjCatChaOnlineShop.Services.Function
{

    public class ChatHub : Hub
    {
        private readonly cachaContext _cachaContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public static List<User> users = new List<User>();
        public static string AdminName = "管理员名字";

        public ChatHub (cachaContext cachaContext, IHttpContextAccessor httpContextAccessor)
        {
            _cachaContext = cachaContext;
            _httpContextAccessor = httpContextAccessor;
        }

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

            string decodedStr = System.Net.WebUtility.HtmlDecode(user);


            var bannedUsers = BannedID();
            bool isBanned = bannedUsers.Any(x => x.Name == decodedStr);

            if (!isBanned)
            {

                await Clients.All.SendAsync("ReceiveMessage", user, message);
            }
            else
            {

                await Clients.Caller.SendAsync("ReceiveMessage", "系統", "您已違反社群發言規則，詳情請聯絡客服人員");
            }
        }

    }

}
