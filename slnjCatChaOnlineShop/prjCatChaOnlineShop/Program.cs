using Microsoft.EntityFrameworkCore;
using prjCatChaOnlineShop.Models;
using prjCatChaOnlineShop.Services.Function;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ImageService>();


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

    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Index}/{action=Index}/{id?}");

});

app.Run();
