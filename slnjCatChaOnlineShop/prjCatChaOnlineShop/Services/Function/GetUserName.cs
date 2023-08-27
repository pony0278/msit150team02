using prjCatChaOnlineShop.Models.ViewModels;
using System.Reflection;
using prjCatChaOnlineShop.Models;
using prjCatChaOnlineShop.Models.CDictionary;
using System.Text.Json;

namespace prjCatChaOnlineShop.Services.Function
{
    public class GetUserName
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public GetUserName(IHttpContextAccessor httpContextAccessor)
        {
    
            _httpContextAccessor = httpContextAccessor;
        }

        //找到會員名稱
        public string? GetCurrentMemberName()
        {
            var memberInfoJson = _httpContextAccessor.HttpContext?.Session.GetString(CDictionary.SK_LOINGED_USER);
            if (memberInfoJson != null)
            {
                var memberInfo = JsonSerializer.Deserialize<ShopMemberInfo>(memberInfoJson);

                return memberInfo.Name;
            }
            return null;
        }

    }
}
