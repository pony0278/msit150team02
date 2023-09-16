using prjCatChaOnlineShop.Models;
using prjCatChaOnlineShop.Models.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Hangfire;


namespace prjCatChaOnlineShop.Services.Function
{
    public class UpdateDatabaseJob
    {
        private readonly cachaContext _cachaContext;
        private readonly ILogger<UpdateDatabaseJob> _logger;
        public UpdateDatabaseJob(cachaContext cachaContext, ILogger<UpdateDatabaseJob> logger)
        {
            _cachaContext = cachaContext;
            _logger = logger;
        }
        public Task Execute()
        {
            try
            {
                _logger.LogInformation("Execute method started."); // 记录信息

                var currentDateTime = DateTime.Now;

                var data = _cachaContext.ShopMemberInfo
                    .Where(x => x.UnBannedTime != null)
                    .ToList();


                foreach (var item in data)
                {
                    if (item.UnBannedTime <= currentDateTime)
                    {
                        item.IsBanned = false;
                    }
                }

                _cachaContext.SaveChanges();


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while executing the job."); // 记录错误信息
            }
            return Task.CompletedTask;
        }
    }
}
