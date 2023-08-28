using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using prjCatChaOnlineShop.Models;
using prjCatChaOnlineShop.Models.CDictionary;

namespace prjCatChaOnlineShop.Controllers
{
    public class SuperController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            if (!HttpContext.Session.Keys.Contains(CDictionary.SK_LOINGED_USER))
            {
                context.Result = RedirectToAction("Login", "MemberLogin");
                //TODO 之後可以調整成登入後回到遊戲大廳，不需要再回去首頁
            }
        }
    }
}
