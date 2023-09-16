using Day20.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;
using prjCatChaOnlineShop.Models;
using prjCatChaOnlineShop.Services.Function;
using System.Net.Http.Headers;

namespace prjCatChaOnlineShop.Controllers.Home
{
    public class EcpayController : Controller
    {
        private readonly cachaContext _context;
        private readonly IMemoryCache _cache;

        public EcpayController(cachaContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }
        public IActionResult Index()
        {
            string userName = HttpContext.Session.GetString("UserName");
            ViewBag.UserName = userName;//把使用者名字傳給_Layout
            return View();
        }

        [HttpPost]
        [Route("Ecpay/AddOrders")]
        public string AddOrders([FromBody] get_localStorage json)
        {
            EcpayOrders Orders = new EcpayOrders();
            Orders.MemberId = json.MerchantID;
            Orders.MerchantTradeNo = json.MerchantTradeNo;
            Orders.RtnCode = 0; //未付款
            Orders.RtnMsg = "訂單成功尚未付款";
            Orders.TradeNo = json.MerchantID.ToString();
            Orders.TradeAmt = json.TotalAmount;
            Orders.PaymentDate = Convert.ToDateTime(json.MerchantTradeDate);
            Orders.PaymentType = json.PaymentType;
            Orders.PaymentTypeChargeFee = "0";
            Orders.TradeDate = json.MerchantTradeDate;
            Orders.SimulatePaid = 0;
            _context.EcpayOrders.Add(Orders);
            _context.SaveChanges();
            return "OK";
        }

        // HomeController->Index->ReturnURL所設定的
        [HttpPost]
        [Route("Ecpay/AddPayInfo")]
        public HttpResponseMessage AddPayInfo(JObject info)
        {
            try
            {
                _cache.Set(info.Value<string>("MerchantTradeNo"), info, TimeSpan.FromMinutes(60));
                return ResponseOK();
            }
            catch (Exception e)
            {
                return ResponseError();
            }
        }
        // HomeController->Index->PaymentInfoURL所設定的
        [HttpPost]
        [Route("Ecpay/AddAccountInfo")]
        public HttpResponseMessage AddAccountInfo(JObject info)
        {
            try
            {
                _cache.Set(info.Value<string>("MerchantTradeNo"), info, TimeSpan.FromMinutes(60));
                return ResponseOK();
            }
            catch (Exception e)
            {
                return ResponseError();
            }
        }
        private HttpResponseMessage ResponseError()
        {
            var response = new HttpResponseMessage();
            response.Content = new StringContent("0|Error");
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }
        private HttpResponseMessage ResponseOK()
        {
            var response = new HttpResponseMessage();
            response.Content = new StringContent("1|OK");
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }
    }
}

