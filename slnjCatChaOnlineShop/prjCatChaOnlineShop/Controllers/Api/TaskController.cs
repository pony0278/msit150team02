using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using prjCatChaOnlineShop.Areas.AdminCMS.Models.ViewModels;
using prjCatChaOnlineShop.Models;
using prjCatChaOnlineShop.Models.CDictionary;
using prjCatChaOnlineShop.Models.CModels;
using System.Security.Cryptography;
using System.Text.Json;

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
        public ActionResult Index()
        {
            return View();
        }
      

       
        public ActionResult Create(CMemberGameInfo c)
        {
            //登入時如果會員都沒有紀錄的話，就每一個任務都新增一欄
            try
            {
                var memberInfoJson = _httpContextAccessor.HttpContext?.Session.GetString(CDictionary.SK_LOINGED_USER);
                var memberInfo = JsonSerializer.Deserialize<ShopMemberInfo>(memberInfoJson);
                int _memberId = memberInfo.MemberId;

                var availibaleTask = _context.GameTaskList.Where(x => x.TaskConditionId == 1).ToList();//選出目前啟用的任務
                var unavailibaleTask = _context.GameTaskList.Where(x => x.TaskConditionId == 2).ToList();//選出目前停用的任務
                //bool memberHasTasks = _context.GameMemberTask.Any(t => t.MemberId == _memberId);

                if (availibaleTask != null)
                {
                    foreach (int TId in availibaleTask.Select(task => task.TaskId))
                    {
                        var newTask = new GameMemberTask//把所有啟用任務加進去
                        {
                            MemberId = _memberId,
                            TaskId = TId
                        };
                        _context.GameMemberTask.Add(newTask);
                    }

                }
                //if (unavailibaleTask != null)
                //{
                //    foreach (int TId in unavailibaleTask.Select(task => task.TaskId))
                //    {
                //        var newTask = new GameMemberTask// 把沒啟用的拿掉
                //        {
                //            MemberId = _memberId,
                //            TaskId = TId
                //        };
                //        _context.GameMemberTask.Remove(newTask);
                //    }
                //}

                _context.SaveChanges();

                return Json(new { Id = _memberId });
            }
            catch (Exception ex)
            {
                // 處理異常情況
                return StatusCode(500, "發生錯誤：" + ex.Message);
            }
        }





        // GET: TaskController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: TaskController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {




                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        
    }
}
