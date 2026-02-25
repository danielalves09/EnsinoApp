using EnsinoApp.Data;
using EnsinoApp.Repositories.Campus;
using EnsinoApp.Repositories.Supervisao;
using EnsinoApp.Repositories.Usuarios;
using EnsinoApp.Repositories.Cursos;
using EnsinoApp.Repositories.Turmas;
using EnsinoApp.Repositories.Matricula;
using EnsinoApp.Repositories.Inscricao;
using EnsinoApp.Repositories.Casal;
using EnsinoApp.Repositories.Licao;
using EnsinoApp.Repositories.RelatorioSemanal;
using EnsinoApp.Services.Campus;
using EnsinoApp.Services.Supervisao;
using EnsinoApp.Services.Usuarios;
using EnsinoApp.Services.Cursos;
using EnsinoApp.Services.Turmas;
using EnsinoApp.Services.Matricula;
using EnsinoApp.Services.Inscricao;
using EnsinoApp.Services.Casal;
using EnsinoApp.Services.Licao;
using EnsinoApp.Services.Lider;
using EnsinoApp.Services.Certificado;
using EnsinoApp.Services.Notifications;
using EnsinoApp.Middlewares;
using EnsinoApp.Data.Configurations;
using EnsinoApp.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DinkToPdf;
using DinkToPdf.Contracts;
using Serilog;
using System.Runtime.InteropServices;
using EnsinoApp.Config;

// ======== CONFIGURA SERILOG =========
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Error()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        "Logs/erros-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

// ================= CARREGAR DLLS NATIVAS DINKTOPDF ==================
// Define o caminho das DLLs dependendo do SO
string nativeLibPath;
if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    nativeLibPath = Path.Combine(AppContext.BaseDirectory, "libwkhtmltox.dll");
else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
    nativeLibPath = Path.Combine(AppContext.BaseDirectory, "libwkhtmltox.so");
else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
    nativeLibPath = Path.Combine(AppContext.BaseDirectory, "libwkhtmltox.dylib");
else
    throw new PlatformNotSupportedException("Sistema operacional não suportado para DinkToPdf");

// Carrega DLL nativa
var context = new CustomAssemblyLoadContext();
context.LoadUnmanagedLibrary(nativeLibPath);

// =================== SERVIÇOS E DEPENDÊNCIAS =====================
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<EnsinoAppContext>();
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<INotificationService, NotificationService>();

// Repositories
builder.Services.AddScoped<ICampusRepository, CampusRepository>();
builder.Services.AddScoped<ISupervisaoRepository, SupervisaoRepository>();
builder.Services.AddScoped<IUsuariosRepository, UsuariosRepository>();
builder.Services.AddScoped<ICursoRepository, CursoRepository>();
builder.Services.AddScoped<ITurmaRepository, TurmaRepository>();
builder.Services.AddScoped<IMatriculaRepository, MatriculaRepository>();
builder.Services.AddScoped<IInscricaoOnlineRepository, InscricaoOnlineRepository>();
builder.Services.AddScoped<ICasalRepository, CasalRepository>();
builder.Services.AddScoped<ILicaoRepository, LicaoRepository>();
builder.Services.AddScoped<IRelatorioSemanalRepository, RelatorioSemanalRepository>();

// Services
builder.Services.AddScoped<ICampusService, CampusService>();
builder.Services.AddScoped<ISupervisaoService, SupervisaoService>();
builder.Services.AddScoped<IUsuariosService, UsuariosService>();
builder.Services.AddScoped<ICursoService, CursoService>();
builder.Services.AddScoped<ITurmaService, TurmaService>();
builder.Services.AddScoped<IMatriculaService, MatriculaService>();
builder.Services.AddScoped<IInscricaoOnlineService, InscricaoOnlineService>();
builder.Services.AddScoped<ICasalService, CasalService>();
builder.Services.AddScoped<ILicaoService, LicaoService>();
builder.Services.AddScoped<ILiderService, LiderService>();
builder.Services.AddScoped<ICertificadoService, CertificadoService>();
builder.Services.AddScoped<IPdfService, PdfService>();
builder.Services.AddScoped<IRazorViewToStringRenderer, RazorViewToStringRenderer>();

// Identity
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

// DinkToPdf
builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

// Configuração do Kestrel para container
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(80);
});

var app = builder.Build();

// ======== PIPELINE DE MIDDLEWARE ==========
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();