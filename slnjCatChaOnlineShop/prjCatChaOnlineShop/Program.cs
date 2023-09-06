using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using prjCatChaOnlineShop.Models;
using prjCatChaOnlineShop.Models.CModels;
using prjCatChaOnlineShop.Models.ViewModels;
using prjCatChaOnlineShop.Services.Function;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
builder.Services.AddScoped<ImageService>();

builder.Services.AddSession(options => {
    // 設定 Session 的過期時間（以分為單位）
    options.IdleTimeout = TimeSpan.FromDays(3); // 測試:這裡設定為 3 天
});
builder.Services.AddScoped<ProductService>();
// 註冊 CheckoutService 服務
builder.Services.AddScoped<CheckoutService>();

//訪問當前 HTTP 要求的相關資訊，例如 HTTP 上下文、Session、Cookies
builder.Services.AddHttpContextAccessor();

// 生成一個新的隨機金鑰
string randomKey = CKeyGenerator.GenerateRandomKey();
// 將隨機金鑰設置到 IConfiguration 裡
builder.Configuration["ForgetPassword:SecretKey"] = randomKey;
System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
ExcelPackage.LicenseContext = LicenseContext.NonCommercial; 

//==============解決 json too big 問題（Mandy需要的請勿刪~桑Q）
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.WriteIndented = true;
});
//==============解決 json too big 問題（Mandy需要的請勿刪~桑Q）

//註冊session要加這個
builder.Services.AddSession();

//讓網頁可以解析DB資料庫

builder.Services.AddDbContext<cachaContext>(
 options => options.UseSqlServer(builder.Configuration.GetConnectionString("CachaConnection")));
var app = builder.Build();



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();



app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=CMSHome}/{action=Login}/{id?}");
    endpoints.MapHub<ChatHub>("/chatHub");
    endpoints.MapHub<OneToOneHub>("/OneToOneHub");
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Index}/{action=Index}/{id?}");

});

app.Run();
