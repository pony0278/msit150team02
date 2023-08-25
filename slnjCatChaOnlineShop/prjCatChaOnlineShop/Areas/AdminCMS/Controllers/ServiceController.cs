using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjCatChaOnlineShop.Models;

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
            return View();
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


            return Json(new {  });
        }
    }
}
