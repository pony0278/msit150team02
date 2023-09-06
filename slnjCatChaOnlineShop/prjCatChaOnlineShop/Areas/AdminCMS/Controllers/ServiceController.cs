using ChatGPT.Net.DTO.ChatGPTUnofficial;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using prjCatChaOnlineShop.Areas.AdminCMS.Models;
using prjCatChaOnlineShop.Models;
using System.Reflection;

namespace prjCatChaOnlineShop.Controllers.CMS
{
    [Area("AdminCMS")]
    public class ServiceController : Controller
    {
        private readonly cachaContext _context;
        public ServiceController(cachaContext context)
        {
            _context = context;
        }
        public IActionResult Service()
        {
            //判斷是否有登入
            if (HttpContext.Session.Keys.Contains(CAdminLogin.SK_LOGINED_USER))
            {
                // 讀取管理員姓名
                string adminName = HttpContext.Session.GetString("AdminName");

                // 將管理員姓名傳給view
                ViewBag.AdminName = adminName;

                return View();
            }
            return RedirectToAction("Login", "CMSHome");
        }
        public IActionResult ShowComplaintCase()
        {
            var data = (
    from ComplaintCase in _context.ShopMemberComplaintCase

        //撈出客訴狀態
    join ComplaintStatusID in _context.ShopComplaintStatusData on ComplaintCase.ComplaintStatusId equals ComplaintStatusID.ComplaintStatusId

    //撈出申訴類別
    join ComplaintCategoryID in _context.ShopAppealCategoryData on ComplaintCase.ComplaintCategoryId equals ComplaintCategoryID.AppealCategoryId

    //撈出會員姓名
    join memberInfo in _context.ShopMemberInfo on ComplaintCase.MemberId equals memberInfo.MemberId

    select new
    {
        ComplaintCase.ComplaintCaseId,
        ComplaintCase.ComplaintTitle,
        ComplaintCase.CreationTime,
        memberInfo.MemberId,
        memberInfo.Name,
        ComplaintStatusID.ComplaintStatusId,
        ComplaintStatusID.ComplaintStatusName,
        ComplaintCategoryID.AppealCategoryId,
        ComplaintCategoryID.CategoryName,
    }
).ToList();

            return Json(new { data });
        }
        public IActionResult GetCaseDetails(int memberId, int caseId)
        {

            var memberInfo = _context.ShopMemberInfo
            .FirstOrDefault(m => m.MemberId == memberId);

            var caseTotal = _context.ShopMemberComplaintCase
                .FirstOrDefault(o => o.ComplaintCaseId == caseId);

            var caseStatusName = _context.ShopComplaintStatusData
                .FirstOrDefault(pm => pm.ComplaintStatusId == caseTotal.ComplaintStatusId)?.ComplaintStatusName;

            var categoryData = _context.ShopAppealCategoryData
                .FirstOrDefault(sm => sm.AppealCategoryId == caseTotal.ComplaintCategoryId)?.CategoryName;

            var replyData = _context.ShopReplyData.FirstOrDefault(sm => sm.ComplaintCaseId == caseTotal.ComplaintCaseId);

            //因為replyData有可能是空的情況（表示未回覆還沒insert進資料庫），這種方式將避免將可能為 null 的屬性傳遞給 LINQ 查詢，同時在每個步驟進行必要的檢查，以確保避免引發異常。

            var replyAdminName = string.Empty; // 預設值或其他適當的處理

            if (replyData != null)
            {
                var receiverId = replyData.ReceiverIdOfficial;
                var admin = _context.ShopGameAdminData.FirstOrDefault(sm => sm.AdminId == receiverId);

                if (admin != null)
                {
                    replyAdminName = admin.AdminUsername;
                }
            }

            var data = new
            {
                MemberInfo = memberInfo,
                CaseTotal = caseTotal,
                CaseStatusName = caseStatusName,
                CategoryData = categoryData,
                ReplyData = replyData,
                ReplyAdminName = replyAdminName
            };

            return Json(new { data });
        }
        public IActionResult ReplyCase(int id)
        {
            var serviceCase = _context.ShopMemberComplaintCase.FirstOrDefault(o => o.ComplaintCaseId == id);

            if (serviceCase != null)
            {

                //得到資料庫裡所有狀態名稱並放到select option裡面
                //var complaintStatusSelect = _context.ShopComplaintStatusData
                //            .Select(c => c.ComplaintStatusName)
                //            .ToList();

                //得到資料庫裡所有分類名稱並放到select option裡面
                var complaintCategorySelect = _context.ShopAppealCategoryData
                            .Select(c => c.CategoryName)
                            .ToList();

                //得到資料庫裡所有管理員姓名並放到select option裡面
                var adminUsername = _context.ShopGameAdminData
                            .Select(c => c.AdminUsername)
                            .ToList();

                string complaintCategoryName = _context.ShopComplaintStatusData.FirstOrDefault(s => s.ComplaintStatusId == serviceCase.ComplaintStatusId)?.ComplaintStatusName; //得到狀態

                string complaintStatusNames = _context.ShopAppealCategoryData.FirstOrDefault(s => s.AppealCategoryId == serviceCase.ComplaintCategoryId)?.CategoryName; //得到分類名稱

                int? receiver = _context.ShopReplyData.FirstOrDefault(s => s.ComplaintCaseId == serviceCase.ComplaintCaseId)?.ReceiverIdOfficial; //得到回覆者ID

                string recipientContent = _context.ShopReplyData.FirstOrDefault(s => s.ComplaintCaseId == serviceCase.ComplaintCaseId)?.MessageRecipientContent; //得到回覆內容

                var data = new
                {
                    ServiceCase = serviceCase,
                    //ComplaintStatusSelect = complaintStatusSelect,
                    ComplaintCategorySelect = complaintCategorySelect,
                    AdminUsername = adminUsername,
                    ComplaintCategoryName = complaintCategoryName,
                    ComplaintStatusNames = complaintStatusNames,
                    Receiver = receiver,
                    RecipientContent = recipientContent
                };

                return Json(new { data });
            }
            else
            {
                return NotFound();
            }
        }
        [HttpPost]
        public JsonResult InsertReply(CCase editCase)
        {
            try
            {
                // 確保插入的回覆資料的 ComplaintCaseId 是有效的案件 ID
                var existingCase = _context.ShopMemberComplaintCase.FirstOrDefault(c => c.ComplaintCaseId == editCase.ComplaintCaseId);
                if (existingCase == null)
                {
                    return Json(new { success = false, message = "案件記錄不存在，無法插入回覆。" });
                }

                // 執行新增 Shop.Reply 資料的操作
                ShopReplyData newReply = new ShopReplyData
                {
                    ComplaintCaseId = editCase.ComplaintCaseId,
                    MessageRecipientContent = editCase.MessageRecipientContent, // 假設你想使用 recipientContent 作為回覆內容
                    ReceiverIdOfficial = editCase.ReceiverIdOfficial,
                    SentTime = DateTime.Now
                };

                _context.ShopReplyData.Add(newReply);
                _context.SaveChanges();

                //=====================其他欄位
                var caseData = _context.ShopMemberComplaintCase.FirstOrDefault(m => m.ComplaintCaseId == editCase.ComplaintCaseId);
                if (caseData != null) // 檢查 orderData 是否為空
                {
                    caseData.ComplaintCaseId = editCase.ComplaintCaseId;
                    caseData.ComplaintStatusId = editCase.ComplaintStatusId;
                    caseData.ComplaintCategoryId = editCase.ComplaintCategoryId;

                    // 存入DbContext
                    _context.SaveChanges();
                }

                return Json(new { success = true, message = "回覆新增成功" });
            }
            catch (Exception ex)
            {
                // 為了除錯，記錄例外狀況
                Console.WriteLine("錯誤：" + ex.Message);

                // 包含內部例外狀況訊息
                string innerExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : "";

                return Json(new { success = false, message = "回覆新增失敗：" + ex.Message + " 內部例外：" + innerExceptionMessage });
            }
        }
        public IActionResult CustomerResponse()
        {
            //判斷是否有登入
            if (HttpContext.Session.Keys.Contains(CAdminLogin.SK_LOGINED_USER))
            {
                // 讀取管理員姓名
                string adminName = HttpContext.Session.GetString("AdminName");

                // 將管理員姓名傳給view
                ViewBag.AdminName = adminName;

                return View();
            }
            return RedirectToAction("Login", "CMSHome");
        }
    }
}
