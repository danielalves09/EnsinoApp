using EnsinoApp.Data;
using EnsinoApp.Repositories.Campus;
using EnsinoApp.Repositories.Supervisao;
using EnsinoApp.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EnsinoApp.Services.Campus;
using EnsinoApp.Services.Supervisao;
using EnsinoApp.Repositories.Usuarios;
using EnsinoApp.Services.Usuarios;
using EnsinoApp.Repositories.Cursos;
using EnsinoApp.Services.Cursos;
using EnsinoApp.Repositories.Turmas;
using EnsinoApp.Services.Turmas;
using EnsinoApp.Repositories.Matricula;
using EnsinoApp.Repositories.Inscricao;
using EnsinoApp.Services.Matricula;
using EnsinoApp.Services.Inscricao;
using EnsinoApp.Repositories.Casal;
using EnsinoApp.Services.Casal;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<EnsinoAppContext>();

//Repositories
builder.Services.AddScoped<ICampusRepository, CampusRepository>();
builder.Services.AddScoped<ISupervisaoRepository, SupervisaoRepository>();
builder.Services.AddScoped<IUsuariosRepository, UsuariosRepository>();
builder.Services.AddScoped<ICursoRepository, CursoRepository>();
builder.Services.AddScoped<ITurmaRepository, TurmaRepository>();
builder.Services.AddScoped<IMatriculaRepository, MatriculaRepository>();
builder.Services.AddScoped<IInscricaoOnlineRepository, InscricaoOnlineRepository>();
builder.Services.AddScoped<ICasalRepository, CasalRepository>();


//Services
builder.Services.AddScoped<ICampusService, CampusService>();
builder.Services.AddScoped<ISupervisaoService, SupervisaoService>();
builder.Services.AddScoped<IUsuariosService, UsuariosService>();
builder.Services.AddScoped<ICursoService, CursoService>();
builder.Services.AddScoped<ITurmaService, TurmaService>();
builder.Services.AddScoped<IMatriculaService, MatriculaService>();
builder.Services.AddScoped<IInscricaoOnlineService, InscricaoOnlineService>();
builder.Services.AddScoped<ICasalService, CasalService>();


builder.Services.AddIdentity<Usuario, IdentityRole<int>>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequiredLength = 6;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
})
.AddEntityFrameworkStores<EnsinoAppContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Auth/Login";
    options.AccessDeniedPath = "/Auth/Login";
});


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

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
