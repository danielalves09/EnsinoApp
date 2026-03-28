using DinkToPdf;
using DinkToPdf.Contracts;
using EnsinoApp.Config;
using EnsinoApp.Data;
using EnsinoApp.Data.Configurations;
using EnsinoApp.Middlewares;
using EnsinoApp.Models.Entities;
using EnsinoApp.Repositories.Campus;
using EnsinoApp.Repositories.Casal;
using EnsinoApp.Repositories.Cursos;
using EnsinoApp.Repositories.Inscricao;
using EnsinoApp.Repositories.Licao;
using EnsinoApp.Repositories.Matricula;
using EnsinoApp.Repositories.RelatorioSemanal;
using EnsinoApp.Repositories.Supervisao;
using EnsinoApp.Repositories.Turmas;
using EnsinoApp.Repositories.Usuarios;
using EnsinoApp.Services.Campus;
using EnsinoApp.Services.Casal;
using EnsinoApp.Services.Certificado;
using EnsinoApp.Services.Cursos;
using EnsinoApp.Services.Email;
using EnsinoApp.Services.Inscricao;
using EnsinoApp.Services.Licao;
using EnsinoApp.Services.Lider;
using EnsinoApp.Services.Matricula;
using EnsinoApp.Services.Notifications;
using EnsinoApp.Services.Supervisao;
using EnsinoApp.Services.Turmas;
using EnsinoApp.Services.Usuarios;
using EnsinoApp.Services.Util;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Runtime.InteropServices;

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
string nativeLibPath;
if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    nativeLibPath = Path.Combine(AppContext.BaseDirectory, "libwkhtmltox.dll");
else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
    nativeLibPath = Path.Combine(AppContext.BaseDirectory, "libwkhtmltox.so");
else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
    nativeLibPath = Path.Combine(AppContext.BaseDirectory, "libwkhtmltox.dylib");
else
    throw new PlatformNotSupportedException("Sistema operacional não suportado para DinkToPdf");

var context = new CustomAssemblyLoadContext();
context.LoadUnmanagedLibrary(nativeLibPath);


// ==================== DATA PROTECTION - IIS =====================
var keysFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "EnsinoApp", "Keys");
Directory.CreateDirectory(keysFolder);

builder.Services.AddDataProtection()
   .PersistKeysToFileSystem(new DirectoryInfo(keysFolder))
   .SetApplicationName("EnsinoApp");


// =================== SERVIÇOS E DEPENDÊNCIAS =====================
builder.Services.AddControllersWithViews();
builder.Services.AddDbContextPool<EnsinoAppContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("EnsinoAppConnection"),
        sqlOptions =>
        {
            sqlOptions.CommandTimeout(30);
            sqlOptions.EnableRetryOnFailure(3);
        }
    )
);
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.AddHttpContextAccessor();

// [ADICIONADO] Compressão de resposta — reduz o tamanho dos dados enviados ao cliente externo
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

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
builder.Services.AddScoped<EnsinoApp.Repositories.Agenda.IAgendaRepository,
                            EnsinoApp.Repositories.Agenda.AgendaRepository>();

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
builder.Services.AddScoped<IUtilService, UtilService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<EnsinoApp.Services.Email.IEmailLembreteService,
                            EnsinoApp.Services.Email.SmtpEmailService>();
builder.Services.AddScoped<EnsinoApp.Services.Agenda.IAgendaService,
EnsinoApp.Services.Agenda.AgendaService>();



// =================== EMAIL =====================
builder.Services.Configure<EnsinoApp.Services.Email.SmtpSettings>(
    builder.Configuration.GetSection("SmtpSettings"));
builder.Services.Configure<EmailSettings>(
builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IEmailTemplateService, EmailTemplateService>();

// Background Service (singleton gerenciado pelo host) - Disparo dos Lembretes de Agenda
builder.Services.AddHostedService<EnsinoApp.Services.Lembrete.LembreteBackgroundService>();

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

// [ATUALIZADO] ForwardedHeaders configurado para confiar no IIS como proxy reverso local
// Com IP fixo, apenas a rede interna como proxy confiável (mais seguro)
var forwardedOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
};
// Remove as restrições padrão
forwardedOptions.KnownNetworks.Clear();
forwardedOptions.KnownProxies.Clear();
//IP interno do servidor onde o IIS está rodando
forwardedOptions.KnownProxies.Add(System.Net.IPAddress.Parse("192.168.1.12"));
app.UseForwardedHeaders(forwardedOptions);

// ExceptionMiddleware para capturar todos os erros
app.UseMiddleware<ExceptionMiddleware>();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // app.UseHsts(); — mantido comentado, reativar quando tiver domínio com HTTPS
}

// app.UseHttpsRedirection(); — mantido comentado, reativar quando tiver domínio com HTTPS


app.UseResponseCompression();

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Landing}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
