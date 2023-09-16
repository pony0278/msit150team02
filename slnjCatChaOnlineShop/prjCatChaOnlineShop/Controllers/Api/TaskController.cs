using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using prjCatChaOnlineShop.Areas.AdminCMS.Models.ViewModels;
using prjCatChaOnlineShop.Models;
using prjCatChaOnlineShop.Models.CDictionary;
using prjCatChaOnlineShop.Models.CModels;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;

namespace prjCatChaOnlineShop.Controllers.Api
{
    public class TaskController : Controller
    {
        private readonly cachaContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TaskController(cachaContext context,  IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        
        // GET: TaskController
        public IActionResult Index()
        {
            return View();
        }


        //登入時如果會員都沒有紀錄的話，就每一個任務都新增一欄
        public IActionResult Create(CMemberGameInfo c)
        {
           
            try
            {
                var memberInfoJson = _httpContextAccessor.HttpContext?.Session.GetString(CDictionary.SK_LOINGED_USER);
                var memberInfo = JsonSerializer.Deserialize<ShopMemberInfo>(memberInfoJson);
                int _memberId = memberInfo.MemberId;


                var availibaleTask = _context.GameTaskList.Where(x => x.TaskConditionId == 1).ToList();//選出目前啟用的任務
                var taskIdList = availibaleTask.Select(task => task.TaskId).ToList();
                /* var unavailibaleTask = _context.GameTaskList.Where(x => x.TaskConditionId == 2).ToList();*///選出目前停用的任務
                                                                                         

                if (taskIdList != null)
                {
                    foreach (var taskId in taskIdList) 
                    {
                        var existingRecord = _context.GameMemberTask.FirstOrDefault(x => x.MemberId == _memberId && x.TaskId == taskId);

                        if (existingRecord == null)
                        {
                            var newTask = new GameMemberTask//把所有啟用任務加進去
                            {
                                TaskProgress = 0,
                                MemberId = _memberId,
                                TaskId = taskId
                            };
                            _context.GameMemberTask.Add(newTask);
                        }
                    }
                }
                _context.SaveChanges();

                return Json(new { Id = _memberId });
            }
            catch (Exception ex)
            {
                // 處理異常情況
                return StatusCode(500, "發生錯誤：" + ex.Message);
            }
        }

        //載入任務
        public IActionResult LoadTask(CMemberTask c)
        {
            try
            {//選出目前啟用的任務

                var memberInfoJson = _httpContextAccessor.HttpContext?.Session.GetString(CDictionary.SK_LOINGED_USER);
                var memberInfo = JsonSerializer.Deserialize<ShopMemberInfo>(memberInfoJson);
                int _memberId = memberInfo.MemberId;
                var availibaleTask = (from p in _context.GameTaskList
                                      where p.TaskConditionId == 1
                                      join i in _context.GameMemberTask
                                      on p.TaskId equals i.TaskId
                                      where i.MemberId == _memberId
                                      orderby p.TaskId descending

                             select new
                             {
                                 p.TaskId,
                                 p.TaskName,
                                 p.TaskReward,
                                 p.TaskRequireTime,
                                 i.TaskProgress,
                                 i.MemberId,
                                 i.CompleteDate
                             })
                             .GroupBy(task => task.TaskId) 
                             .Select(group => group.First())
                             .ToList();
                if (availibaleTask.Any())
                {

                    return new JsonResult(availibaleTask);
                }

                    return NotFound();
            }
            catch
            {
                return View();
            }
        }


        //更新任務狀態
        [HttpPost]
        public IActionResult UpdateTask([FromBody]GameMemberTask g) 
        {
        
            try
            {
                var memberInfoJson = _httpContextAccessor.HttpContext?.Session.GetString(CDictionary.SK_LOINGED_USER);
                var memberInfo = JsonSerializer.Deserialize<ShopMemberInfo>(memberInfoJson);
                int _memberId = memberInfo.MemberId;

                var teargetTask = _context.GameMemberTask.FirstOrDefault(x => x.MemberId == _memberId && x.TaskId == g.TaskId);
                var task = _context.GameTaskList.FirstOrDefault(x => x.TaskId == g.TaskId);

                if( teargetTask != null)
                {
                    if(teargetTask.CompleteDate==null)
                    teargetTask.TaskProgress ++ ;
                    _context.SaveChanges();

                    if (task != null) 
                    {
                        var taskrequiretime = task.TaskRequireTime;
                        if (teargetTask.TaskProgress == taskrequiretime)
                        {
                            teargetTask.CompleteDate = DateTime.Now;
                        }
                        _context.SaveChanges();
                    }
                    
                    return new JsonResult(teargetTask);
                }

                    return NotFound();
            }
            catch
            {
                return View();
            }
        }



        //更新任務狀態
        [HttpPost]
        public IActionResult ResetTaskAfterReward([FromBody] GameMemberTask g)
        {
            try
            {
                var memberInfoJson = _httpContextAccessor.HttpContext?.Session.GetString(CDictionary.SK_LOINGED_USER);
                var memberInfo = JsonSerializer.Deserialize<ShopMemberInfo>(memberInfoJson);
                int _memberId = memberInfo.MemberId;

                var teargetTask = _context.GameMemberTask.FirstOrDefault(x => x.MemberId == _memberId && x.TaskId == g.TaskId);
                var task = _context.GameTaskList.FirstOrDefault(x => x.TaskId == g.TaskId);

                if (teargetTask != null)
                {
                    if (task != null)
                    {
                        var taskrequiretime = task.TaskRequireTime;
                        if (teargetTask.TaskProgress == taskrequiretime)
                        {
                            teargetTask.CompleteDate = null;
                        }
                        _context.SaveChanges();
                    }

                    return new JsonResult(teargetTask);
                }

                return NotFound();
            }
            catch
            {
                return View();
            }
        }


        public IActionResult demoRest([FromBody] GameMemberTask g) {
            try
            {
                var memberInfoJson = _httpContextAccessor.HttpContext?.Session.GetString(CDictionary.SK_LOINGED_USER);
                var memberInfo = JsonSerializer.Deserialize<ShopMemberInfo>(memberInfoJson);
                int _memberId = memberInfo.MemberId;

                var thismember = _context.ShopMemberInfo.OrderBy(x => x.MemberId).LastOrDefault(x => x.MemberId == _memberId);

                thismember.LastLoginTime = DateTime.Now.AddDays(-1);
                _context.SaveChanges();
                    return new JsonResult(thismember.LastLoginTime);   
            }
            catch
            {
                return View();
            }

        }


        public IActionResult showTutorial([FromBody] GameMemberTask g)
        {
            try
            {
                var memberInfoJson = _httpContextAccessor.HttpContext?.Session.GetString(CDictionary.SK_LOINGED_USER);
                var memberInfo = JsonSerializer.Deserialize<ShopMemberInfo>(memberInfoJson);
                int _memberId = memberInfo.MemberId;

                var thismember = _context.ShopMemberInfo.OrderBy(x => x.MemberId).LastOrDefault(x => x.MemberId == _memberId);

                if (thismember.RunGameHighestScore==0)
                    return new JsonResult(true);
                return new JsonResult(false);
            }
            catch
            {
                return View();
            }

        }

    }
}
