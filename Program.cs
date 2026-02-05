using EnsinoApp.Data;
using EnsinoApp.Repositories.Campus;
using EnsinoApp.Repositories.Supervisao;
using EnsinoApp.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EnsinoApp.Services.Campus;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<EnsinoAppContext>();
builder.Services.AddScoped<ICampusRepository, CampusRepository>();
builder.Services.AddScoped<ISupervisaoRepository, SupervisaoRepository>();
builder.Services.AddScoped<ICampusService, CampusService>();


builder.Services.AddIdentity<Usuario, IdentityRole<int>>()
    .AddEntityFrameworkStores<EnsinoAppContext>()
    .AddDefaultTokenProviders();



var app = builder.Build();



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
