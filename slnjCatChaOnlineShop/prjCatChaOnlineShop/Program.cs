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
    // �]�w Session ���L���ɶ��]�H�������^
    options.IdleTimeout = TimeSpan.FromDays(3); // ����:�o�̳]�w�� 3 ��
});
builder.Services.AddScoped<ProductService>();
// ���U CheckoutService �A��
builder.Services.AddScoped<CheckoutService>();

//�X�ݷ�e HTTP �n�D��������T�A�Ҧp HTTP �W�U��BSession�BCookies
builder.Services.AddHttpContextAccessor();

// �ͦ��@�ӷs���H�����_
string randomKey = CKeyGenerator.GenerateRandomKey();
// �N�H�����_�]�m�� IConfiguration ��
builder.Configuration["ForgetPassword:SecretKey"] = randomKey;
System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
ExcelPackage.LicenseContext = LicenseContext.NonCommercial; 

//==============�ѨM json too big ���D�]Mandy�ݭn���ФŧR~��Q�^
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.WriteIndented = true;
});
//==============�ѨM json too big ���D�]Mandy�ݭn���ФŧR~��Q�^

//���Usession�n�[�o��
builder.Services.AddSession();

//�������i�H�ѪRDB��Ʈw

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
