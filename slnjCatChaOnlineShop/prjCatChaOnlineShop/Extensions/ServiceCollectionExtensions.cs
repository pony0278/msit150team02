﻿using prjCatChaOnlineShop.Models;
using prjCatChaOnlineShop.Services.Function;

namespace prjCatChaOnlineShop.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMyServices(this IServiceCollection services)
        {
            //填入你要註冊約束的泛型與類別
            //services.AddScoped<IContentService<Content>, ContentService>();
            //services.AddScoped<>();
            /*services.AddScoped<GetUserName>(); */// 註冊 GetUserName 服務
            return services;
        }

    }
}
