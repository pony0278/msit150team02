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
        public  Task Execute()
        {
            try
            {
                _logger.LogInformation("Execute method started."); // 记录信息


                var data = _cachaContext.ShopMemberInfo.Where(x => x.UnBannedTime <= DateTime.Now).ToList();


                if (data.Count > 0)
                {
                    _logger.LogInformation($"Found {data.Count} records to update."); // 如果找到数据则记录信息

                    foreach (var item in data)
                    {
                        item.IsBanned = false;
                    }

                    _cachaContext.SaveChanges();

                }
                else
                {
                    _logger.LogInformation("No records found to update."); // 如果没有找到数据则记录信息
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while executing the job."); // 记录错误信息
            }
            return Task.CompletedTask;
        }
    }
}
