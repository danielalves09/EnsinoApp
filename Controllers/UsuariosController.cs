using EnsinoApp.Models.Entities;
using EnsinoApp.Services.Campus;
using EnsinoApp.Services.Notifications;
using EnsinoApp.Services.Usuarios;
using EnsinoApp.ViewModels.Usuario;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EnsinoApp.Controllers;

public class UsuariosController : Controller
{

    private readonly IUsuariosService _usuarioService;
    private readonly UserManager<Usuario> _userManager;
    private readonly SignInManager<Usuario> _signInManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;
    private readonly IWebHostEnvironment _env;
    private readonly INotificationService _notification;
    private const int TAMANHO_PAGINA = 10;

    public UsuariosController(UserManager<Usuario> userManager, RoleManager<IdentityRole<int>> roleManager, IUsuariosService usuarioService, IWebHostEnvironment env, INotificationService notification, SignInManager<Usuario> signInManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _usuarioService = usuarioService;
        _env = env;
        _notification = notification;
        _signInManager = signInManager;
    }

    [Authorize(Roles = "Admin,Pastor,Coordenador,Supervisor")]
    public IActionResult Index(string filtro)
    {
        var usuarios = _usuarioService.FindAll();

        if (!string.IsNullOrWhiteSpace(filtro))
        {
            filtro = filtro.ToLower();

            usuarios = usuarios.Where(u =>
                u.Email!.ToLower().Contains(filtro) ||
                u.NomeMarido.ToLower().Contains(filtro) ||
                u.NomeEsposa.ToLower().Contains(filtro)
            );
        }

        var model = usuarios.Select(u => new ListarUsuarioViewModel
        {
            Id = u.Id,
            Email = u.Email!,
            NomeMarido = u.NomeMarido,
            NomeEsposa = u.NomeEsposa,
            Campus = u.Campus.Nome,
            Supervisao = u.Supervisao?.Nome ?? "-",
            Ativo = u.Ativo
        });

        return View(model);
    }

    [Authorize(Roles = "Admin,Pastor,Coordenador,Supervisor")]
    public IActionResult Adicionar()
    {
        return View(new AdicionarUsuarioViewModel());

    }

    [Authorize(Roles = "Admin,Pastor,Coordenador,Supervisor")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Adicionar(AdicionarUsuarioViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var usuario = new Usuario
        {
            UserName = model.Email,
            Email = model.Email,
            IdCampus = model.IdCampus,
            NomeMarido = model.NomeMarido ?? string.Empty,
            NomeEsposa = model.NomeEsposa ?? string.Empty,
            IdSupervisao = model.IdSupervisao,
            Ativo = model.Ativo,
            DataCriacao = DateTime.Now
        };

        var resultado = await _userManager.CreateAsync(usuario, model.Senha);

        if (!resultado.Succeeded)
        {
            foreach (var erro in resultado.Errors)
                ModelState.AddModelError("", erro.Description);

            return View(model);
        }

        // Criar role se não existir
        if (!await _roleManager.RoleExistsAsync(model.Role))
        {
            await _roleManager.CreateAsync(new IdentityRole<int>(model.Role));
        }

        await _userManager.AddToRoleAsync(usuario, model.Role);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Buscar(string filtro)
    {
        var usuarios = _usuarioService.FindAll()
            .Where(u => u.NomeMarido.ToLower().Contains(filtro.ToLower()) || u.NomeEsposa.ToLower().Contains(filtro.ToLower()))
            .Select(u => new
            {
                u.Id,
                Nome = $"{u.NomeMarido} / {u.NomeEsposa}",
                u.Email,
                NomeCampus = u.Campus.Nome
            })
            .ToList();

        return Json(usuarios);
    }

    [Authorize(Roles = "Admin,Pastor,Coordenador,Supervisor, Lider")]
    public async Task<IActionResult> Perfil()
    {
        var user = await _userManager.GetUserAsync(User);
        var usuario = user != null ? _usuarioService.FindById(user.Id) : null;
        var model = new UsuarioPerfilViewModel
        {
            Nome = user!.NomeMarido,
            Email = user.Email,
            FotoPerfilUrl = user.FotoPerfil,
            Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? "Não definida",
            Campus = usuario!.Campus.Nome,
            Supervisao = usuario.Supervisao?.Nome
        };

        return View(model);
    }

    [Authorize(Roles = "Admin,Pastor,Coordenador,Supervisor, Lider")]
    [HttpPost]
    public async Task<IActionResult> AlterarFoto(UsuarioPerfilViewModel model)
    {
        if (model.Foto == null || model.Foto.Length == 0)
        {
            _notification.Warning("Selecione uma imagem válida.");
            return RedirectToAction(nameof(Perfil));
        }

        var user = await _userManager.GetUserAsync(User);

        var pasta = Path.Combine(_env.WebRootPath, "uploads", "perfis");
        if (!Directory.Exists(pasta))
            Directory.CreateDirectory(pasta);

        var extensao = Path.GetExtension(model.Foto.FileName);

        // fallback de segurança
        if (string.IsNullOrEmpty(extensao))
            extensao = ".png";

        var nomeArquivo = $"usuario_{user.Id}{extensao}";
        var caminhoFisico = Path.Combine(pasta, nomeArquivo);

        using (var stream = new FileStream(caminhoFisico, FileMode.Create))
        {
            await model.Foto.CopyToAsync(stream);
        }

        user.FotoPerfil = $"/uploads/perfis/{nomeArquivo}";
        await _userManager.UpdateAsync(user);

        _notification.Success("Foto atualizada com sucesso.");
        return RedirectToAction(nameof(Perfil));
    }

    [Authorize(Roles = "Admin,Pastor,Coordenador,Supervisor, Lider")]
    [HttpPost]
    public async Task<IActionResult> AlterarSenha(UsuarioPerfilViewModel model)
    {

        var user = await _userManager.GetUserAsync(User);

        var result = await _userManager.ChangePasswordAsync(
            user,
            model.SenhaAtual,
            model.NovaSenha
        );

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        await _signInManager.RefreshSignInAsync(user);
        _notification.Success("Senha alterada com sucesso.");

        return RedirectToAction(nameof(Perfil));
    }

    [Authorize(Roles = "Admin,Pastor,Coordenador,Supervisor, Lider")]
    [HttpPost]
    public async Task<IActionResult> AtualizarDados(UsuarioPerfilViewModel model)
    {

        var user = await _userManager.GetUserAsync(User);

        user.NomeMarido = model.Nome;
        user.Email = model.Email;
        user.UserName = model.Email;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            _notification.Error("Erro ao atualizar dados.");
            return View(model);
        }

        await _signInManager.RefreshSignInAsync(user);
        _notification.Success("Dados atualizados com sucesso.");

        return RedirectToAction(nameof(Perfil));
    }

}